# Datarium API
 
API em **.NET 8** para gerenciar **portfólios de investimento**.  
Usa **PostgreSQL + EF Core**, armazena os ativos em **JSONB** e tem documentação no **Swagger**.
 
---
 
## ⚙️ Requisitos
 
- .NET 8 SDK  
- PostgreSQL 14+  
- Pacotes NuGet:
  - Microsoft.EntityFrameworkCore  
  - Npgsql.EntityFrameworkCore.PostgreSQL  
  - Swashbuckle.AspNetCore  
 
---
 
## 🔧 Configuração
 
**appsettings.json**
 
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=datarium;Username=postgres;Password=SUA_SENHA;"
  }
}

// Banco de dados 

CREATE TABLE portfolios (
    id BIGSERIAL PRIMARY KEY,
    name VARCHAR(160) NOT NULL,
    riskprofile INT NOT NULL,
    totalamount NUMERIC(18,2) NOT NULL,
    assets JSONB NOT NULL
);
