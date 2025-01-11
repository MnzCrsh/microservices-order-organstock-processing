# Microservices Architecture with CQRS, Event Sourcing, and Saga Pattern

This project demonstrates a microservices architecture using CQRS, Event Sourcing, Apache Kafka, and the Saga pattern. It simulates a simple order processing system with payment handling, event-driven architecture, and compensating transactions in case of failures.

## High-Level Architecture

The diagram below shows the interaction between services in the system:

```mermaid
graph TD
    subgraph Client
        A[Client] -->|Create Order Command| B[Order Service]
        A -->|Fetch Order Query| B
    end

    subgraph Order
        B[Order Service] -->|OrderCreatedEvent| C[Payment Service]
        B -->|OrderPaidEvent| A
    end

    subgraph Payment
        C[Payment Service] -->|Process Payment Command| D[Payment Gateway]
        D[Payment Gateway] -->|PaymentProcessedEvent| C
        C -->|PaymentProcessedEvent| B
    end

    subgraph Saga Orchestrator
        E[Cancel Order Command] -->|OrderCancelledEvent| B
        F[Cancel Payment Command] -->|PaymentCancelledEvent| C
    end

    subgraph Kafka_Event_Bus
        B -.->|OrderCreatedEvent| Kafka[Kafka]
        C -.->|PaymentProcessedEvent| Kafka
        D -.->|PaymentProcessedEvent| Kafka
        Kafka -.->|OrderPaidEvent| A
        Kafka -.->|PaymentCancelledEvent| C
        Kafka -.->|OrderCancelledEvent| B
    end

    subgraph Event storage
        G[Event Store] -->|OrderCreatedEvent| B
        G -->|PaymentProcessedEvent| C
    end

    subgraph Integration
        A -.->|Send Events| Kafka
        Kafka -.->|Events| B
        Kafka -.->|Events| C
    end

    B -.->|Handle Events| E
    C -.->|Handle Events| F

    %% Event refs
    B -->|OrderCreatedEvent| G
    C -->|PaymentProcessedEvent| G
    B -->|OrderPaidEvent| Kafka
    C -->|PaymentProcessedEvent| Kafka
