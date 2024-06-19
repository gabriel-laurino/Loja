import requests
import json


urlToken = "http://localhost:5085/login"

payloadToken = {
    "login": "admin",
    "senha": "admin",
}

dataToken = json.dumps(payloadToken)
headersToken = {"Content-Type": "application/json"}

response = requests.post(urlToken, data=dataToken, headers=headersToken)

if response.status_code == 200:
    json_response = response.json()
    token = json_response.get("token")
    print(f"Login bem sucedido! \n\nToken: {token}")
else:
    error_message = response.content.decode("utf-8")
    print("Resposta completa:\n", error_message)
    first_line = error_message.split("\n")[0]
    print("Erro simplificado:\n", first_line)
    token = ""


if token:
    urlProdutos = "http://localhost:5085/produtos"
    headersProdutos = {"Content-Type": "application/json", "Authorization": f"Bearer {token}"}

    try:
        responseProdutos = requests.get(urlProdutos, headers=headersProdutos)
        responseProdutos.raise_for_status()

        if responseProdutos.status_code == 200:
            produtos = responseProdutos.json()
            print("\nProdutos obtidos com sucesso:")
            for produto in produtos:
                print(produto)
        else:
            error_message = responseProdutos.content.decode("utf-8")
            print("\nResposta completa:\n", error_message)
            first_line = error_message.split("\n")[0]
            print("\nErro simplificado:\n", first_line)
    except requests.exceptions.RequestException as e:
        print(f"\nErro na requisição para produtos: {e}")
else:
    print("\nNão foi possível obter o token para a requisição de produtos.")
