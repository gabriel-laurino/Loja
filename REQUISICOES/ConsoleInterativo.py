import requests
import json

##############################################################################################
# INSERT INTO Usuarios (Login, Senha, Email)
#     VALUES ('Admin', '$2a$11$xQCiA03rTK8m.pz1niXjpuEVggCk/BicQxZrhEwvYxs942gDrVYOG', 'Admin@admin.com'); 
##############################################################################################

BASE_URL = "http://localhost:5085"
LOGIN_URL = f"{BASE_URL}/login"
ADMIN_CREDENTIALS = {
    "login": "Admin",
    "senha": "Admin"
}

def get_token():
    response = requests.post(LOGIN_URL, data=json.dumps(ADMIN_CREDENTIALS), headers={"Content-Type": "application/json"})
    if response.status_code == 200:
        return response.json().get("token")
    else:
        print("\nErro ao fazer login como administrador")
        print(response.text)
        return None

def get_all_clients(token):
    url = f"{BASE_URL}/clientes"
    headers = {"Authorization": f"Bearer {token}"}
    response = requests.get(url, headers=headers)
    if response.status_code == 200:
        clientes = response.json()
        print("\nClientes disponíveis:")
        for cliente in clientes:
            print(f"ID: {cliente['id']}, Nome: {cliente['nome']}")
        return {cliente['id']: cliente['nome'] for cliente in clientes}
    else:
        print("\nErro ao obter lista de clientes")
        print(response.text)
        return {}

def get_all_servicos(token):
    url = f"{BASE_URL}/servicos"
    headers = {"Authorization": f"Bearer {token}"}
    response = requests.get(url, headers=headers)
    if response.status_code == 200:
        servicos = response.json()
        print("\nServiços disponíveis:")
        for servico in servicos:
            print(f"ID: {servico['id']}, Nome: {servico['nome']}")
        return {servico['id']: servico['nome'] for servico in servicos}
    else:
        print("\nErro ao obter lista de serviços")
        print(response.text)
        return {}

def create_servico(token):
    url = f"{BASE_URL}/servicos"
    nome = input("\nDigite o nome do serviço: ")
    preco = float(input("Digite o preço do serviço: "))
    status = input("Digite o status do serviço (True/False): ").lower() == 'true'
    
    servico = {
        "Nome": nome,
        "Preco": preco,
        "Status": status
    }
    headers = {"Content-Type": "application/json", "Authorization": f"Bearer {token}"}
    response = requests.post(url, data=json.dumps(servico), headers=headers)
    if response.status_code == 201:
        print("\nServiço criado com sucesso!")
    else:
        print("\nErro ao criar serviço")
        print(response.text)

def update_servico(token):
    url = f"{BASE_URL}/servicos/{int(input('Digite o ID do serviço: '))}"
    nome = input("\nDigite o nome do serviço: ")
    preco = float(input("Digite o preço do serviço: "))
    status = input("Digite o status do serviço (True/False): ").lower() == 'true'
    
    servico = {
        "Nome": nome,
        "Preco": preco,
        "Status": status
    }
    headers = {"Content-Type": "application/json", "Authorization": f"Bearer {token}"}
    response = requests.put(url, data=json.dumps(servico), headers=headers)
    if response.status_code == 200:
        print("\nServiço atualizado com sucesso!")
    else:
        print("\nErro ao atualizar serviço")
        print(response.text)

def get_servico(token):
    url = f"{BASE_URL}/servicos/{int(input('Digite o ID do serviço: '))}"
    headers = {"Authorization": f"Bearer {token}"}
    response = requests.get(url, headers=headers)
    if response.status_code == 200:
        servico = response.json()
        print(servico)
    else:
        print("\nErro ao consultar serviço")
        print(response.text)

def create_contrato(token):
    clientes = get_all_clients(token)
    if not clientes:
        return

    cliente_id = int(input("\nDigite o ID do cliente: "))
    if cliente_id not in clientes:
        print("ID de cliente inválido.")
        return

    servicos = get_all_servicos(token)
    if not servicos:
        return

    servico_id = int(input("Digite o ID do serviço: "))
    if servico_id not in servicos:
        print("ID de serviço inválido.")
        return

    preco_cobrado = float(input("Digite o preço cobrado: "))
    data_contratacao = input("Digite a data de contratação (YYYY-MM-DD): ")
    
    contrato = {
        "ClienteId": cliente_id,
        "ServicoId": servico_id,
        "PrecoCobrado": preco_cobrado,
        "DataContratacao": data_contratacao
    }
    headers = {"Content-Type": "application/json", "Authorization": f"Bearer {token}"}
    response = requests.post(f"{BASE_URL}/contratos", data=json.dumps(contrato), headers=headers)
    if response.status_code == 201:
        print("\nContrato criado com sucesso!")
    else:
        print("\nErro ao criar contrato")
        print(response.text)

def get_servicos_contratados_por_cliente(token):
    clientes = get_all_clients(token)
    if not clientes:
        return

    cliente_id = int(input("\nDigite o ID do cliente: "))
    if cliente_id not in clientes:
        print("\nID de cliente inválido.")
        return

    url = f"{BASE_URL}/clientes/{cliente_id}/servicos"
    headers = {"Authorization": f"Bearer {token}"}
    response = requests.get(url, headers=headers)
    if response.status_code == 200:
        servicos = response.json()
        print("")
        for servico in servicos:
            print(servico)
    else:
        print("\nErro ao consultar serviços contratados por cliente")
        print(response.text)

def create_usuario(token):
    url = f"{BASE_URL}/usuarios"
    login = input("\nDigite o login do usuário: ")
    senha = input("Digite a senha do usuário: ")
    email = input("Digite o email do usuário: ")
    
    usuario = {
        "Login": login,
        "Senha": senha,
        "Email": email
    }
    headers = {"Content-Type": "application/json", "Authorization": f"Bearer {token}"}
    response = requests.post(url, data=json.dumps(usuario), headers=headers)
    if response.status_code == 201:
        print("\nUsuário criado com sucesso!")
    else:
        print("\nErro ao criar usuário")
        print(response.text)

def main():
    token = get_token()
    if not token:
        return
    
    while True:
        print("\nEscolha uma opção:")
        print("1. Criar Serviço")
        print("2. Atualizar Serviço")
        print("3. Consultar Serviço")
        print("4. Registrar Contrato")
        print("5. Consultar Serviços Contratados por Cliente")
        print("6. Criar Usuário")
        print("7. Sair")

        choice = input("\nDigite sua escolha: ")

        if choice == "1":
            create_servico(token)
        elif choice == "2":
            update_servico(token)
        elif choice == "3":
            get_servico(token)
        elif choice == "4":
            create_contrato(token)
        elif choice == "5":
            get_servicos_contratados_por_cliente(token)
        elif choice == "6":
            create_usuario(token)
        elif choice == "7":
            break
        else:
            print("\nOpção inválida. Tente novamente.")

if __name__ == "__main__":
    main()