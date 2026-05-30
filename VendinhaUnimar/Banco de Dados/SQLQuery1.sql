CREATE TABLE Clientes (
    Id           INT IDENTITY(1,1) NOT NULL,
    NomeCompleto NVARCHAR(100)     NOT NULL,
    Cpf          VARCHAR(11)       NOT NULL,
    DataNascimento DATE            NOT NULL,
    Email        VARCHAR(100)      NULL,
    PRIMARY KEY (Id),
    UNIQUE (Cpf)
);
GO

-- CRIA TABELA DE DIVIDAS
CREATE TABLE Dividas (
    Id             INT IDENTITY(1,1) NOT NULL,
    ClienteId      INT               NOT NULL,
    Valor          DECIMAL(10, 2)    NOT NULL,
    Situacao       VARCHAR(10)       NOT NULL DEFAULT 'Aberta',
    DataCriacao    DATETIME2         NOT NULL,
    DataPagamento  DATETIME2         NULL,
    PRIMARY KEY (Id),
    FOREIGN KEY (ClienteId) REFERENCES Clientes(Id) ON DELETE CASCADE
);
GO

-- DADOS DE EXEMPLO
INSERT INTO Clientes (NomeCompleto, Cpf, DataNascimento, Email)
VALUES
('Jose da Silva',   '52998224725', '2000-01-01', 'jose@email.com'),
('Fulano de Tal',   '11144477735', '1999-05-15', 'fulano@gmail.com');
GO

INSERT INTO Dividas (ClienteId, Valor, Situacao, DataCriacao, DataPagamento)
VALUES
(1, 150.00, 'Aberta', GETDATE(), NULL),
(2, 200.50, 'Paga',   GETDATE(), GETDATE());
GO

SELECT * FROM Clientes;
SELECT * FROM Dividas;
GO
