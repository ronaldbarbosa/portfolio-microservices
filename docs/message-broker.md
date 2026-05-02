# Message Broker com RabbitMQ e MassTransit

## O problema que o message broker resolve

Em uma arquitetura de microserviços, os serviços precisam se comunicar. A forma mais óbvia é a chamada HTTP direta (síncrona):

```
Portfolio.Project  →  HTTP POST  →  Portfolio.ImageUploader
```

Isso funciona, mas cria um **acoplamento síncrono**: se o ImageUploader estiver fora do ar no momento da chamada, a operação do Project falha junto — mesmo que as responsabilidades dos dois serviços sejam independentes.

O message broker resolve isso introduzindo um intermediário:

```
Portfolio.Project  →  [mensagem]  →  RabbitMQ  →  [mensagem]  →  Portfolio.ImageUploader
```

O Project publica uma mensagem e segue em frente. O ImageUploader consome essa mensagem quando estiver disponível. Os dois serviços nunca se falam diretamente.

---

## Conceitos fundamentais

### Mensagem / Evento
Uma mensagem é um objeto serializado que representa algo que aconteceu. Neste projeto, `ProjectDeleted` é um **evento** — notifica que um projeto foi deletado, sem exigir resposta.

```csharp
public record ProjectDeleted(long Id, string ImageUrl);
```

A diferença entre **evento** e **comando**:
- **Evento**: "isso aconteceu" — o produtor não sabe quem vai consumir
- **Comando**: "faça isso" — direcionado a um destinatário específico

### Producer (Produtor)
Serviço que publica mensagens. `Portfolio.Project` publica `ProjectDeleted` quando um projeto é deletado. Não sabe se alguém vai consumir, nem quando.

### Consumer (Consumidor)
Serviço que assina e processa mensagens. `Portfolio.ImageUploader` consome `ProjectDeleted` e deleta a imagem do Cloudinary.

### Exchange
Componente do RabbitMQ que recebe mensagens dos produtores e as roteia para as filas. O MassTransit cria e configura exchanges automaticamente com base no tipo da mensagem.

### Queue (Fila)
Buffer onde mensagens aguardam ser processadas. Se o consumidor estiver fora do ar, as mensagens ficam na fila até ele voltar — nenhuma mensagem é perdida.

---

## RabbitMQ

RabbitMQ é um broker de mensagens open source que implementa o protocolo **AMQP** (Advanced Message Queuing Protocol). Ele age como o intermediário entre produtores e consumidores.

### Interface de gerenciamento

Com o Docker Compose deste projeto, o RabbitMQ sobe com uma interface web acessível em:

```
http://localhost:15672
usuário: guest
senha: guest
```

Na interface é possível visualizar exchanges, filas, mensagens em trânsito e estatísticas de throughput.

---

## MassTransit

MassTransit é uma abstração sobre brokers de mensagens (RabbitMQ, Azure Service Bus, Amazon SQS, etc). Ele gerencia:

- Serialização/desserialização de mensagens
- Criação automática de exchanges e filas no broker
- Retry em caso de falha no consumidor
- Correlação de mensagens (tracing)

Sem MassTransit, seria necessário configurar manualmente exchanges, filas, bindings e serialização no RabbitMQ. Com MassTransit, basta registrar o consumer e ele cuida do resto.

---

## Implementação neste projeto

### Contrato compartilhado (`SharedContracts`)

O evento é definido em uma biblioteca separada, referenciada por ambos os serviços:

```
SharedContracts/
└── Events/
    └── ProjectDeleted.cs   ← record com Id e ImageUrl
```

Ambos os serviços referenciam `SharedContracts.csproj`, garantindo que produtor e consumidor usem exatamente o mesmo tipo.

### Producer — `Portfolio.Project`

Configuração em `Program.cs` (apenas publica, sem consumers):

```csharp
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((_, cfg) =>
    {
        cfg.Host(new Uri(rabbitMqConnectionString!));
    });
});
```

Publicação em `DeleteProject.cs`:

```csharp
await publishEndpoint.Publish(new ProjectDeleted(project.Id, project.Image), cancellationToken);
```

`IPublishEndpoint` é injetado automaticamente pelo MassTransit via DI.

### Consumer — `Portfolio.ImageUploader`

Configuração em `Program.cs` (registra o consumer, que cria a fila automaticamente):

```csharp
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ProjectDeletedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri(rabbitMqConnectionString!));
        cfg.ConfigureEndpoints(context); // cria a fila automaticamente
    });
});
```

Consumer em `Consumers/ProjectDeletedConsumer.cs`:

```csharp
public class ProjectDeletedConsumer : IConsumer<ProjectDeleted>
{
    public async Task Consume(ConsumeContext<ProjectDeleted> context)
    {
        await storageService.DeleteAsync(context.Message.ImageUrl, context.CancellationToken);
    }
}
```

### Fluxo completo

```
DELETE /api/projects/42
        ↓
ProjectRepository.DeleteAsync(project)
        ↓
publishEndpoint.Publish(new ProjectDeleted(42, "https://res.cloudinary.com/..."))
        ↓
     RabbitMQ
        ↓
ProjectDeletedConsumer.Consume(...)
        ↓
CloudinaryImageStorageService.DeleteAsync("https://res.cloudinary.com/...")
        ↓
Imagem removida do Cloudinary
```

O `DELETE /api/projects/42` retorna `204 No Content` imediatamente após deletar o projeto e publicar o evento — sem esperar a deleção da imagem.

---

## Configuração por ambiente

### Rider (local)

O RabbitMQ precisa estar rodando. A forma mais simples é subir só ele via Docker:

```bash
docker compose up rabbitmq -d
```

A connection string está em `appsettings.Development.json`:

```json
"RabbitMq": {
  "ConnectionString": "amqp://guest:guest@localhost:5672/"
}
```

### Docker Compose (local completo)

O serviço `rabbitmq` está declarado no `docker-compose.yml`. Ambos os serviços aguardam o health check do RabbitMQ antes de iniciar (`depends_on: condition: service_healthy`).

A connection string é injetada via variável de ambiente:

```
RABBITMQ_CONNECTION_STRING=amqp://guest:guest@rabbitmq:5672/
```

### Render (produção)

O RabbitMQ não pode rodar no Render free tier. Use o **CloudAMQP**, que tem um plano gratuito com 1 milhão de mensagens por mês.

1. Acesse [cloudamqp.com](https://www.cloudamqp.com) e crie uma instância gratuita (plano **Little Lemur**)
2. Copie a AMQP URL fornecida (formato: `amqps://user:pass@host/vhost`)
3. No dashboard do Render, defina a variável `RABBITMQ_CONNECTION_STRING` nos dois serviços (`project-service` e `image-uploader-service`)

---

## Testando o fluxo

Com todos os serviços rodando:

1. Crie um projeto via `POST /api/projects` com uma imagem
2. Delete o projeto via `DELETE /api/projects/{id}`
3. Verifique no dashboard do RabbitMQ (`http://localhost:15672`) que a mensagem foi publicada e consumida
4. Confirme no Cloudinary que a imagem foi removida

Para inspecionar mensagens sem consumi-las, use a aba **Queues** na interface do RabbitMQ e clique em **Get Messages**.
