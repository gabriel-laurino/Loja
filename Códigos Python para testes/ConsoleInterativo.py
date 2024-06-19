import requests
import json
import uuid

BASE_URL = "http://localhost:5085"
LOGIN_URL = f"{BASE_URL}/login"
ADMIN_CREDENTIALS = {
    "login": "admin",
    "senha": "admin"
}

def get_token():
    response = requests.post(LOGIN_URL, data=json.dumps(ADMIN_CREDENTIALS), headers={"Content-Type": "application/json"})
    if response.status_code == 200:
        return response.json().get("token")
    else:
        print("Erro ao fazer login como administrador")
        print(response.text)
        return None

def create_venda(token):
    url = f"{BASE_URL}/vendas"
    cliente_id = int(input("Digite o ID do cliente: "))
    produto_id = int(input("Digite o ID do produto: "))
    quantidade_vendida = int(input("Digite a quantidade vendida: "))
    numero_nota_fiscal = input("Digite o número da nota fiscal (ou deixe vazio para gerar automaticamente): ")

    if not numero_nota_fiscal:
        numero_nota_fiscal = str(uuid.uuid4())

    venda = {
        "ClienteId": cliente_id,
        "ProdutoId": produto_id,
        "QuantidadeVendida": quantidade_vendida,
        "NumeroNotaFiscal": numero_nota_fiscal
    }
    headers = {"Content-Type": "application/json", "Authorization": f"Bearer {token}"}
    response = requests.post(url, data=json.dumps(venda), headers=headers)
    if response.status_code == 201:
        print("Venda criada com sucesso!")
    else:
        print("Erro ao criar venda")
        print(response.text)

def get_vendas_by_produto_detalhada(token):
    produto_id = int(input("Digite o ID do produto: "))
    url = f"{BASE_URL}/vendas/produto/detalhada/{produto_id}"
    headers = {"Authorization": f"Bearer {token}"}
    response = requests.get(url, headers=headers)
    if response.status_code == 200:
        vendas = response.json()
        for venda in vendas:
            print(venda)
    else:
        print("Erro ao consultar vendas por produto (detalhada)")
        print(response.text)

def get_vendas_by_produto_sumarizada(token):
    produto_id = int(input("Digite o ID do produto: "))
    url = f"{BASE_URL}/vendas/produto/sumarizada/{produto_id}"
    headers = {"Authorization": f"Bearer {token}"}
    response = requests.get(url, headers=headers)
    if response.status_code == 200:
        vendas = response.json()
        for venda in vendas:
            print(venda)
    else:
        print("Erro ao consultar vendas por produto (sumarizada)")
        print(response.text)

def get_vendas_by_cliente_detalhada(token):
    cliente_id = int(input("Digite o ID do cliente: "))
    url = f"{BASE_URL}/vendas/cliente/detalhada/{cliente_id}"
    headers = {"Authorization": f"Bearer {token}"}
    response = requests.get(url, headers=headers)
    if response.status_code == 200:
        vendas = response.json()
        for venda in vendas:
            print(venda)
    else:
        print("Erro ao consultar vendas por cliente (detalhada)")
        print(response.text)

def get_vendas_by_cliente_sumarizada(token):
    cliente_id = int(input("Digite o ID do cliente: "))
    url = f"{BASE_URL}/vendas/cliente/sumarizada/{cliente_id}"
    headers = {"Authorization": f"Bearer {token}"}
    response = requests.get(url, headers=headers)
    if response.status_code == 200:
        vendas = response.json()
        for venda in vendas:
            print(venda)
    else:
        print("Erro ao consultar vendas por cliente (sumarizada)")
        print(response.text)

def main():
    token = get_token()
    if not token:
        return
    
    while True:
        print("\nEscolha uma opção:")
        print("1. Criar Venda")
        print("2. Consultar Vendas por Produto (Detalhada)")
        print("3. Consultar Vendas por Produto (Sumarizada)")
        print("4. Consultar Vendas por Cliente (Detalhada)")
        print("5. Consultar Vendas por Cliente (Sumarizada)")
        print("6. Sair")

        choice = input("Digite sua escolha: ")

        if choice == "1":
            create_venda(token)
        elif choice == "2":
            get_vendas_by_produto_detalhada(token)
        elif choice == "3":
            get_vendas_by_produto_sumarizada(token)
        elif choice == "4":
            get_vendas_by_cliente_detalhada(token)
        elif choice == "5":
            get_vendas_by_cliente_sumarizada(token)
        elif choice == "6":
            break
        else:
            print("Opção inválida. Tente novamente.")

if __name__ == "__main__":
    main()
