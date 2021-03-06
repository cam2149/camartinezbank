USE [AppApayments]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Transacciones_Cuentas]') AND parent_object_id = OBJECT_ID(N'[dbo].[Transactions]'))
ALTER TABLE [dbo].[Transactions] DROP CONSTRAINT [FK_Transacciones_Cuentas]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Transacciones_Clientes]') AND parent_object_id = OBJECT_ID(N'[dbo].[Transactions]'))
ALTER TABLE [dbo].[Transactions] DROP CONSTRAINT [FK_Transacciones_Clientes]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Cuentas_Clientes]') AND parent_object_id = OBJECT_ID(N'[dbo].[Accounts]'))
ALTER TABLE [dbo].[Accounts] DROP CONSTRAINT [FK_Cuentas_Clientes]
GO
/****** Object:  UserDefinedFunction [dbo].[customer_transactions]    Script Date: 17/11/2020 8:02:05 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[customer_transactions]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[customer_transactions]
GO
/****** Object:  Table [dbo].[Transactions]    Script Date: 17/11/2020 8:02:05 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Transactions]') AND type in (N'U'))
DROP TABLE [dbo].[Transactions]
GO
/****** Object:  Table [dbo].[Parameters]    Script Date: 17/11/2020 8:02:05 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Parameters]') AND type in (N'U'))
DROP TABLE [dbo].[Parameters]
GO
/****** Object:  Table [dbo].[Clients]    Script Date: 17/11/2020 8:02:05 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND type in (N'U'))
DROP TABLE [dbo].[Clients]
GO
/****** Object:  Table [dbo].[Accounts]    Script Date: 17/11/2020 8:02:05 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Accounts]') AND type in (N'U'))
DROP TABLE [dbo].[Accounts]
GO
/****** Object:  StoredProcedure [dbo].[apply_gmf_client]    Script Date: 17/11/2020 8:02:05 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[apply_gmf_client]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[apply_gmf_client]
GO
/****** Object:  StoredProcedure [dbo].[adjust_ account_balance]    Script Date: 17/11/2020 8:02:05 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[adjust_ account_balance]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[adjust_ account_balance]
GO
/****** Object:  StoredProcedure [dbo].[adjust_ account_balance]    Script Date: 17/11/2020 8:02:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[adjust_ account_balance]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		<Carlos Alberto Martinez R.>
-- Create date: <2020-11-15>
-- Description:	<Calculate and apply the GMF amount to the account for financial movements>
-- =============================================
CREATE procedure [dbo].[adjust_ account_balance]
	   @id_transaction  uniqueidentifier = null output
as
	DECLARE
	@type_transaction varchar(12),
	@type_movement varchar(12),
	@amount_movement decimal(18,4)

	set implicit_transactions off
	if(@id_transaction is null )
		begin
			RAISERROR (''Invalid ARG'', 16, 1)
			return -1
	end
	
	set nocount on

	BEGIN TRANSACTION

	SELECT @type_movement = t.type_movement, @type_transaction = t.type_transaction, @amount_movement = t.transaction_amount FROM Transactions t WHERE id_transaction = @id_transaction

	
	if (@type_transaction = ''CREDIT'') 
	begin
		UPDATE [dbo].[Accounts] SET  current_account_balance = current_account_balance  - Transactions.transaction_amount
		 FROM Accounts INNER JOIN Transactions ON Accounts.id_account = Transactions.id_account AND Accounts.id_customer = Transactions.id_customer
         WHERE bank_account = Transactions.source_bank AND Transactions.id_transaction = @id_transaction;
	end


	if (@type_transaction = ''DEBIT'' ) 
	begin
		UPDATE [dbo].[Accounts] SET  current_account_balance = current_account_balance  + Transactions.transaction_amount
		 FROM Accounts INNER JOIN Transactions ON Accounts.id_account = Transactions.id_account AND Accounts.id_customer = Transactions.id_customer
         WHERE Accounts.bank_account = Transactions.source_bank AND Transactions.id_transaction = @id_transaction;
	end


	if @@error <> 0
	begin
		ROLLBACK TRANSACTION
		return (-1)
	end
	
	COMMIT TRANSACTION

	return 1

' 
END
GO
/****** Object:  StoredProcedure [dbo].[apply_gmf_client]    Script Date: 17/11/2020 8:02:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[apply_gmf_client]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE procedure [dbo].[apply_gmf_client]
	   @id_transaction  uniqueidentifier = null output
as
	DECLARE
	@fechaini_mes datetime,
	@fechafin_mes datetime,
	@montomaximo_gmf decimal,
	@acumulado_gmf decimal,
	@acumulado_diario_gmf bit,
	@acumuladotransacciones decimal(18, 4),
	@valor_gmf decimal(18, 4),
    @fecha_transaccion datetime,
    @valor_transaccion decimal(18, 4),
    @id_cliente	uniqueidentifier,
    @banco varchar(10),
    @tipo_transaccion varchar(10),
	@tipo_movimiento varchar(10)

	set implicit_transactions off
	if(@id_transaction is null)
		begin
			RAISERROR (''Invalid ARG'', 16, 1)
			return -1
	end
	
	set nocount on

	BEGIN TRANSACTION

	SELECT @fecha_transaccion = t.date_transaction, @valor_transaccion = t.transaction_amount, @banco = t.source_bank,
	@tipo_transaccion = t.type_transaction, @id_cliente = t.id_customer
	FROM Transactions t WHERE t.id_transaction = @id_transaction;

	if(@tipo_transaccion <> ''CREDIT'')
		begin
			RAISERROR (''Invalid ARG'', 16, 1)
			return -1
	end

	Select @fechaini_mes = DATEADD(MM,DATEDIFF(MM, 0, @fecha_transaccion),0) --First day of the month 
	Select @fechafin_mes = DATEADD(MINUTE,59,DATEADD(HOUR,23,DATEADD(MM,DATEDIFF(MM, -1, @fecha_transaccion),-1)))

	SELECT @montomaximo_gmf = maximum_transaction_amount, @acumulado_diario_gmf = daily_gmf_calculation FROM [dbo].[Parameters];
	SELECT @acumuladotransacciones = SUM(tc.transaction_amount) FROM dbo.customer_transactions(@fecha_transaccion, @id_cliente, @tipo_transaccion, @banco) as tc;
  
    if (@acumuladotransacciones > @montomaximo_gmf)
	begin
		
		SET @valor_gmf  = ((@acumuladotransacciones - @montomaximo_gmf)*4)/1000;
		
		UPDATE dbo.Transactions SET valuebasegmf_transaccion = (@acumuladotransacciones - @montomaximo_gmf),
		gmf_transaction_value = @valor_gmf
		WHERE id_transaction = @id_transaction AND id_customer = @id_cliente;

	end
	
	if @@error <> 0
	begin
		ROLLBACK TRANSACTION
		return (1)
	end
	COMMIT TRANSACTION


	return 1

' 
END
GO
/****** Object:  Table [dbo].[Accounts]    Script Date: 17/11/2020 8:02:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Accounts]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Accounts](
	[id_account] [uniqueidentifier] NOT NULL,
	[number_account] [nvarchar](50) NOT NULL,
	[id_customer] [uniqueidentifier] NOT NULL,
	[current_account_balance] [decimal](18, 4) NOT NULL,
	[bank_account] [nvarchar](10) NOT NULL,
	[exemption_gmf] [bit] NOT NULL,
 CONSTRAINT [PK_BBVA_Cuentas] PRIMARY KEY CLUSTERED 
(
	[id_account] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Clients]    Script Date: 17/11/2020 8:02:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Clients](
	[id_customer] [uniqueidentifier] NOT NULL,
	[customer_code] [nvarchar](50) NULL,
	[customer_name] [varchar](50) NULL,
	[customer_lastname] [varchar](50) NULL,
 CONSTRAINT [PK_Clientes] PRIMARY KEY CLUSTERED 
(
	[id_customer] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Parameters]    Script Date: 17/11/2020 8:02:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Parameters]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Parameters](
	[id_parameters] [uniqueidentifier] NOT NULL,
	[maximum_transaction_amount] [decimal](18, 4) NOT NULL,
	[daily_gmf_calculation] [bit] NOT NULL,
 CONSTRAINT [PK_Parameters] PRIMARY KEY CLUSTERED 
(
	[id_parameters] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Transactions]    Script Date: 17/11/2020 8:02:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Transactions]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Transactions](
	[id_transaction] [uniqueidentifier] NOT NULL,
	[date_transaction] [datetime] NOT NULL,
	[transaction_amount] [decimal](18, 4) NOT NULL,
	[valuebasegmf_transaccion] [decimal](18, 4) NOT NULL,
	[gmf_transaction_value] [decimal](18, 4) NOT NULL,
	[destination_account] [uniqueidentifier] NULL,
	[id_account] [uniqueidentifier] NOT NULL,
	[id_customer] [uniqueidentifier] NOT NULL,
	[source_bank] [nvarchar](10) NOT NULL,
	[destination_bank] [nchar](10) NULL,
	[type_transaction] [nvarchar](10) NOT NULL,
	[type_movement] [nvarchar](12) NOT NULL,
 CONSTRAINT [PK_Transacciones] PRIMARY KEY CLUSTERED 
(
	[id_transaction] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  UserDefinedFunction [dbo].[customer_transactions]    Script Date: 17/11/2020 8:02:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[customer_transactions]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
BEGIN
execute dbo.sp_executesql @statement = N'-- =============================================
-- Author:		<CARLOS ALBERTO MARTINEZ>
-- Create date: <2020-11-14>
-- Description:	<Extrae todas las Transactions de un cliente por
-- @date fecha del movimiento,
-- @id_customer,
-- @type_transaction (DEBIT - CREDIT - TRANSFER)
-- @bank >
-- =============================================
CREATE FUNCTION [dbo].[customer_transactions]
(	
		@date datetime,
		@id_customer uniqueidentifier,
		@type_transaction varchar(10),
		@bank varchar(10)
)
RETURNS TABLE 
AS
RETURN 
(
	
	SELECT t.*
	FROM [dbo].[Transactions] t CROSS JOIN [dbo].[Parameters] p
	WHERE t.id_customer = @id_customer
	AND t.type_transaction = @type_transaction
	AND t.date_transaction BETWEEN 
	CASE WHEN (p.daily_gmf_calculation = 1) THEN 
		   DATEADD(day, DATEDIFF(day, 0, @date ), 0)
		ELSE
			DATEADD(MM,DATEDIFF(MM, 0, @date),0)
		END
		AND 
		CASE WHEN (p.daily_gmf_calculation = 1) THEN 
			DATEADD(MINUTE,59,DATEADD(HOUR,23,DATEADD(day, DATEDIFF(day, 0, @date ), 0)))
		ELSE
			DATEADD(MINUTE,59,DATEADD(HOUR,23,DATEADD(MM,DATEDIFF(MM, -1, @date),-1)))
		END
  AND t.source_bank = @bank
)
' 
END

GO
INSERT [dbo].[Accounts] ([id_account], [number_account], [id_customer], [current_account_balance], [bank_account], [exemption_gmf]) VALUES (N'e4946ebb-aaaa-406b-bd72-852f03356cb4', N'A-2025-94525376', N'e4946ebb-b5cc-406b-bd72-852f03356cb4', CAST(60000000.0000 AS Decimal(18, 4)), N'1', 0)
GO
INSERT [dbo].[Accounts] ([id_account], [number_account], [id_customer], [current_account_balance], [bank_account], [exemption_gmf]) VALUES (N'e4946ebb-bbbb-406b-bd72-852f03356cb4', N'B-2025-94525376', N'e4946ebb-b5cc-406b-bd72-852f03356cb4', CAST(65000000.0000 AS Decimal(18, 4)), N'2', 0)
GO
INSERT [dbo].[Accounts] ([id_account], [number_account], [id_customer], [current_account_balance], [bank_account], [exemption_gmf]) VALUES (N'e4946ebb-cccc-406b-bd72-852f03356cb4', N'C-2025-94525376', N'e4946ebb-b5cc-406b-bd72-852f03356cb4', CAST(5500000.0000 AS Decimal(18, 4)), N'2', 0)
GO
INSERT [dbo].[Clients] ([id_customer], [customer_code], [customer_name], [customer_lastname]) VALUES (N'e4946ebb-b5cc-406b-bd72-852f03356cb4', N'1103000027', N'Jennifer', N'Villa')
GO
INSERT [dbo].[Parameters] ([id_parameters], [maximum_transaction_amount], [daily_gmf_calculation]) VALUES (N'e5970ebb-b5cc-406b-bd72-852f03356cb4', CAST(9600000.0000 AS Decimal(18, 4)), 1)
GO
INSERT [dbo].[Transactions] ([id_transaction], [date_transaction], [transaction_amount], [valuebasegmf_transaccion], [gmf_transaction_value], [destination_account], [id_account], [id_customer], [source_bank], [destination_bank], [type_transaction], [type_movement]) VALUES (N'92a80ba4-8132-4e86-80fb-3a239ddab671', CAST(0x0000AC68001BBD90 AS DateTime), CAST(5000000.0000 AS Decimal(18, 4)), CAST(0.0000 AS Decimal(18, 4)), CAST(0.0000 AS Decimal(18, 4)), N'e4946ebb-cccc-406b-bd72-852f03356cb4', N'e4946ebb-cccc-406b-bd72-852f03356cb4', N'e4946ebb-b5cc-406b-bd72-852f03356cb4', N'2', N'2         ', N'DEBIT', N'3')
GO
INSERT [dbo].[Transactions] ([id_transaction], [date_transaction], [transaction_amount], [valuebasegmf_transaccion], [gmf_transaction_value], [destination_account], [id_account], [id_customer], [source_bank], [destination_bank], [type_transaction], [type_movement]) VALUES (N'92a80ba4-8132-4e86-80fb-3a239ddab672', CAST(0x0000AC68001BBD90 AS DateTime), CAST(10000000.0000 AS Decimal(18, 4)), CAST(400000.0000 AS Decimal(18, 4)), CAST(1600.0000 AS Decimal(18, 4)), N'e4946ebb-aaaa-406b-bd72-852f03356cb4', N'e4946ebb-aaaa-406b-bd72-852f03356cb4', N'e4946ebb-b5cc-406b-bd72-852f03356cb4', N'1', N'1         ', N'CREDIT', N'2')
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Cuentas_Clientes]') AND parent_object_id = OBJECT_ID(N'[dbo].[Accounts]'))
ALTER TABLE [dbo].[Accounts]  WITH CHECK ADD  CONSTRAINT [FK_Cuentas_Clientes] FOREIGN KEY([id_customer])
REFERENCES [dbo].[Clients] ([id_customer])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Cuentas_Clientes]') AND parent_object_id = OBJECT_ID(N'[dbo].[Accounts]'))
ALTER TABLE [dbo].[Accounts] CHECK CONSTRAINT [FK_Cuentas_Clientes]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Transacciones_Clientes]') AND parent_object_id = OBJECT_ID(N'[dbo].[Transactions]'))
ALTER TABLE [dbo].[Transactions]  WITH CHECK ADD  CONSTRAINT [FK_Transacciones_Clientes] FOREIGN KEY([id_customer])
REFERENCES [dbo].[Clients] ([id_customer])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Transacciones_Clientes]') AND parent_object_id = OBJECT_ID(N'[dbo].[Transactions]'))
ALTER TABLE [dbo].[Transactions] CHECK CONSTRAINT [FK_Transacciones_Clientes]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Transacciones_Cuentas]') AND parent_object_id = OBJECT_ID(N'[dbo].[Transactions]'))
ALTER TABLE [dbo].[Transactions]  WITH CHECK ADD  CONSTRAINT [FK_Transacciones_Cuentas] FOREIGN KEY([id_account])
REFERENCES [dbo].[Accounts] ([id_account])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Transacciones_Cuentas]') AND parent_object_id = OBJECT_ID(N'[dbo].[Transactions]'))
ALTER TABLE [dbo].[Transactions] CHECK CONSTRAINT [FK_Transacciones_Cuentas]
GO
