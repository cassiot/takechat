Olá, primeiramente muito obrigado pela oportunidade.

Trabalhei 7 horas no projeto de chat da Take e fiz o que pude de melhor nesse tempo. Infelizmente eu não posso mais continuar porque tenho obrigações de trabalho para entregar e estou fazendo um curso de Deep Learning com um prazo final marcado, então meu tempo está bem curto.

Utilizei para construir a API o framework WebApi no AspNet Core, versão 3.1

No caso caso do cliente, utilizei uma aplicação console. Boa parte do tempo foi utilizado aqui só por causa dos menus e desenhos de tela. Não acho que seja o ideal mas foi o que aconteceu.



Por causa do motivo acima, gostaria de esclarecer que:

-Eu sei que teria que fazer dezenas, ou centenas de testes e validações.

-Não me preocupei tanto com boas práticas no cliente. Eu sei que poderia ser melhorado.

-Implementei todas operações num único controller já que não são muitas. Talvez num projeto real tivesse que refatorar para 3 controllers. Rooms, Messages, Users

-Coloquei poucos comentário porque acho que o código é auto explicativo em sua maior parte. Não sei se esse é um conceito que a Take usa mas pelas considerações acima, considero suficiente.

Outras considerações:

-Mesmo com essa restrição, acredito que fiz quase tudo que deveria no projeto. Não é o que eu entregaria como um trabalho de médio a longo prazo mas para 7 horas de trabalho acho que está muito bom.

-Fiz o chat funcionar com 3 janelas simultâneas, testei mensagens públicas, para um usuário e privadas e todas funcionaram perfeitamente

-Utilizei o padrão Repository de forma bem simples só para demonstrar alguma separação de dependência para vocês. Mais uma vez, se fosse num projeto maior e mais complexo, o grau de sofistificação deve ser maior.

-Como todo o código roda em memória, o projeto de implementação do repositório não precisa ser duplicado para se fazer testes. No caso, se isso fosse ser transformado em um projeto profissional, o repositório atual seria utilizado para testes e um outro seria criado para servir a aplicação


Para testar a aplicação, basta colocar rodar o projeto da API e depois rodar as janelas de cliente. As instruções de uso aparecem na própria tela.

Obrigado