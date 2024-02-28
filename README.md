# WebCrawlerFineProxy

## Descrição
O WebCrawlerFineProxy é um projeto de web crawler desenvolvido em C# utilizando o Selenium WebDriver para extrair dados de proxies disponíveis no site FineProxy.

## Funcionalidades
- Extrair os campos "IP Address", "Port", "Country" e "Protocol" de todas as linhas, em todas as páginas disponíveis durante a execução do site FineProxy.
- Salvar os dados extraídos em um arquivo JSON local.
- Salvar os metadados da execução no banco de dados SQLite, incluindo as datas de início e término da execução, número de páginas processadas, número de linhas extraídas e o nome do arquivo JSON gerado.
- Salvar um arquivo HTML de cada página para fins de auditoria e depuração.
- Implementação de lógica de multithreading, com um máximo de 3 execuções simultâneas.

## Uso
1. Certifique-se de ter o ambiente de desenvolvimento para C# configurado, incluindo o .NET SDK e o Visual Studio ou Visual Studio Code.
2. Clone este repositório para sua máquina local.
3. Abra o projeto no Visual Studio ou Visual Studio Code.
4. Execute o projeto.
5. Os dados extraídos serão salvos em um arquivo JSON chamado `proxies.json` na raiz do projeto.
6. Os metadados da execução serão salvos no banco de dados SQLite, com o nome `webcrawler_metadata.db`, na raiz do projeto.
7. Os arquivos HTML de cada página serão salvos no diretório `page_prints`, na raiz do projeto.

## Dependências
- Selenium WebDriver: Ferramenta utilizada para automatizar a interação com o navegador web.
- ChromeDriver: WebDriver específico para o navegador Google Chrome.
- System.Data.SQLite: Biblioteca para acesso ao banco de dados SQLite.
- Newtonsoft.Json: Biblioteca para manipulação de dados JSON em C#.

## Configuração do Ambiente
Certifique-se de ter as seguintes ferramentas instaladas e configuradas em seu ambiente de desenvolvimento:
- .NET SDK
- Google Chrome
- WebDriver do Google Chrome (ChromeDriver)
- Visual Studio ou Visual Studio Code (opcional)

## Contribuição
Contribuições são bem-vindas! Sinta-se à vontade para abrir problemas relatando problemas, sugestões ou melhorias. Você também pode enviar solicitações de pull para contribuir com código para este projeto.

## Autor
[Seu Nome]
