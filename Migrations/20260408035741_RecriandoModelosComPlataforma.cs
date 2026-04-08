using Microsoft.EntityFrameworkCore.Migrations;

protected override void Up(MigrationBuilder migrationBuilder)
{
	// 1. Removemos a chave primária antiga baseada no Id
	migrationBuilder.DropPrimaryKey(
		name: "PK_UsuariosJogos",
		table: "UsuariosJogos");

	migrationBuilder.DropIndex(
		name: "IX_UsuariosJogos_UsuarioId",
		table: "UsuariosJogos");

	// 2. Removemos a coluna 'Plataforma' antiga do Jogo (Data loss warning)
	migrationBuilder.DropColumn(
		name: "Plataforma",
		table: "Jogos");

	// --- AQUI ESTÁ O TRUQUE ---
	// Em vez de AlterColumn, nós DROP e ADD
	migrationBuilder.DropColumn(
		name: "Id",
		table: "UsuariosJogos");

	migrationBuilder.AddColumn<int>(
		name: "Id",
		table: "UsuariosJogos",
		type: "int",
		nullable: false);
	// --------------------------

	// 3. Criamos a nova Chave Primária Composta
	migrationBuilder.AddPrimaryKey(
		name: "PK_UsuariosJogos",
		table: "UsuariosJogos",
		columns: new[] { "UsuarioId", "JogoId" });

	// 4. Criação das novas tabelas (Plataformas e Ligação)
	migrationBuilder.CreateTable(
		name: "Plataformas",
		columns: table => new
		{
			Id = table.Column<int>(type: "int", nullable: false)
				.Annotation("SqlServer:Identity", "1, 1"),
			Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
			LogoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
		},
		constraints: table =>
		{
			table.PrimaryKey("PK_Plataformas", x => x.Id);
		});

	migrationBuilder.CreateTable(
		name: "JogoPlataforma",
		columns: table => new
		{
			JogosId = table.Column<int>(type: "int", nullable: false),
			PlataformasId = table.Column<int>(type: "int", nullable: false)
		},
		constraints: table =>
		{
			table.PrimaryKey("PK_JogoPlataforma", x => new { x.JogosId, x.PlataformasId });
			table.ForeignKey(
				name: "FK_JogoPlataforma_Jogos_JogosId",
				column: x => x.JogosId,
				principalTable: "Jogos",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
			table.ForeignKey(
				name: "FK_JogoPlataforma_Plataformas_PlataformasId",
				column: x => x.PlataformasId,
				principalTable: "Plataformas",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		});

	migrationBuilder.CreateIndex(
		name: "IX_JogoPlataforma_PlataformasId",
		table: "JogoPlataforma",
		column: "PlataformasId");
}