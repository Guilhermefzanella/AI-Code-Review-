import os  # import não utilizado (Ruff F401)

# Script de teste para o AI Code Review

def soma(a, b):
    return a+b  # propositalmente sem espaços para violar PEP8

def run(code_str):
    # ⚠ INSEGURO: uso de eval apenas para teste do Bandit/Semgrep
    return eval(code_str)

def slow_sum(n):
    # Implementação ingênua para simular ineficiência (performance)
    s=0
    for i in range(n):
        s=s+i
    return s

if __name__ == "__main__":
    print(soma(5,3))
    print(run("2+2"))
    print(slow_sum(10000))
