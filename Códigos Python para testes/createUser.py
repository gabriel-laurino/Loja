import requests
import json

# Endpoint de login para obtenção do token de administrador
urlTokenAdmin = "http://localhost:5085/login"

payloadTokenAdmin = {
    "login": "admin",
    "senha": "admin",
}

dataTokenAdmin = json.dumps(payloadTokenAdmin)
headersTokenAdmin = {"Content-Type": "application/json"}

responseAdmin = requests.post(urlTokenAdmin, data=dataTokenAdmin, headers=headersTokenAdmin)

if responseAdmin.status_code == 200:
    json_response_admin = responseAdmin.json()
    admin_token = json_response_admin.get("token")
    print(f"\nLogin de administrador bem sucedido! \n\nToken: {admin_token}")
else:
    error_message = responseAdmin.content.decode("utf-8")
    print("\nErro ao fazer login como administrador:\n", error_message)
    admin_token = ""

# Se o login do administrador foi bem-sucedido, criar um novo usuário
if admin_token:
    # Endpoint de criação de usuário
    urlCreateUser = "http://localhost:5085/usuarios"
    
    # Dados do novo usuário
    payloadCreateUser = {
        "login": "userNovo",
        "senha": "123",
    }
    
    dataCreateUser = json.dumps(payloadCreateUser)
    headersCreateUser = {
        "Content-Type": "application/json",
        "Authorization": f"Bearer {admin_token}"
    }
    
    responseCreateUser = requests.post(urlCreateUser, data=dataCreateUser, headers=headersCreateUser)
    
    if responseCreateUser.status_code == 201:
        print("\nUsuario criado com sucesso!")
    else:
        error_message = responseCreateUser.content.decode("utf-8")
        print("\nErro ao criar usuario:\n", error_message)
else:
    print("Nao foi possivel obter o token de administrador.")
