
1. Setup

Este projeto foi feito com docker, os containers são:
 - Backend: Utilizado .NET OpenApi (antigo Swagger) com .NET 9 e Entity Framework Core. Responsavel apenas pela conexão com o Banco de Dados.
 - Frontend: React + TailwindCSS
 - Worker: Utilizado C# com .NET 9, o objetivo desse container é disparar o evento toda vez que um objeto for inserido nas filas do RabbitMQ.
 - Database: PostgreSQL
 - RabbitMQ: Mensageria

2. Execução 

Para a execução basta rodar:
docker compose up

E abrir:
http://127.0.0.1:3000/

3. Detalhes técnicos

O Worker é independente da API, rodando em outro container pois tem motivos diferentes.
A API em backend segue as boas práticas de código SOLID, o Worker não (para mostrar a diferença de um código limpo e bem estruturado de um todo bagunçado).
O frontend é minimo pois meu forte não é ele, e sim o backend.

Fluxo de execução:
 - Abrir aplicação em http://127.0.0.1:3000/
 - Clicar em "Add Order"
 - Preencher os campos
 - Clicar em "Add order"
 $ O frontend faz requisição PUT no backend http://127.0.0.1:8080/API/Test com o json
 $ A classe PedidoController.cs executa o método CreatePedido() com o json do frontend.
 $ Esse método insere os dados no banco de dados e depois insere no RabbitMQ o código do ID do pedido na fila "NewOrder"
 $ O RabbitMQ avisa o Program.cs do Worker que executa o evento NewOrderReceived(), esse evento espera 5 segundos, atualiza o banco utilizando o backend e depois coloca outra mensagem no Rabbit, dessa vez na fila de "OrderProcessed".
 $ O RabbitMQ avisa novamente o Worker, dessa vez executa o evento OrderProcessedReceived(), espera 5 segundos, atualiza o banco novamente utilizando o backend e encerra.