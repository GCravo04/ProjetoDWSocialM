using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MVVM.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Connection String
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddSignalR();

// Identity
// RequireConfirmedAccount a false: não há servidor de email configurador, por isso
// a conta tem de ficar utilizável logo a seguir ao registo.
builder.Services.AddDefaultIdentity<AppUser>(options => { options.SignIn.RequireConfirmedAccount = false; })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Razor Pages
builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Redireciona erros de status (404,403, ...) para a página /Error.
// Fica fora do if acima intencionalmente, para as páginas de erro funcionarem
// também em desenvolvimento e não só depois de publicado.
app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.UseStaticFiles(); // serve as imagens carregadas em runtime (wwwroot/uploads)

app.MapRazorPages()
    .WithStaticAssets();

app.MapHub<FeedHub>("/feedHub");
// Seed de roles e utilizador admin
// Corre em cada arranque porque é preciso garantir que existe sempre um admin:
// numa base de dados vazia não haveria ninguém com permissões para criar o primeiro.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();

    // Garante que os roles existem
    string[] roles = { "Admin", "User" };
    foreach (var role in roles)
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));

    // Garante que existe um utilizador admin
    var adminEmail = "admin@socialm.com";
    var admin = await userManager.FindByEmailAsync(adminEmail);
    if (admin == null)
    {
        admin = new AppUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };
        await userManager.CreateAsync(admin, "Admin123!");
        await userManager.AddToRoleAsync(admin, "Admin");
    }
    else if (!await userManager.IsInRoleAsync(admin, "Admin"))
    {
        await userManager.AddToRoleAsync(admin, "Admin");
    }
}

app.Run();