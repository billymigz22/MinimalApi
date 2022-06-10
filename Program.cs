using Microsoft.EntityFrameworkCore;
using MinimalApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

async Task<List<SuperHero>> GetAllHeroes(DataContext context) => 
    await context.SuperHeroes.ToListAsync();

app.MapGet("/", () => "Welcome to the Super Hero DB! ❤");

app.MapGet("/superhero", async (DataContext context) => {
    //return await context.SuperHeroes
    // .Include(SuperHero => SuperHero.Locations)
    //     .ToListAsync();

    return await context.Locations
        .Include(Location => Location.SuperHero)
        .ToListAsync();

});

app.MapGet("/superhero/{FirstName1}/{Lastname1}", async (DataContext context, string FirstName1, string LastName1) => {
    //await context.SuperHeroes.FindAsync(id) is SuperHero hero ? 
    //    Results.Ok(hero) : 
    //    Results.NotFound("Sorry, Hero Not Found!"));
        
    var result = await context.SuperHeroes
                .Where(x => x.FirstName == FirstName1 && x.LastName == LastName1)
                .ToListAsync();

    return Results.Ok(result);

});

app.MapPost("/superhero", async (DataContext context, SuperHero hero) =>
{
    context.SuperHeroes.Add(hero);
    await context.SaveChangesAsync();
    
    return Results.Ok(await GetAllHeroes(context));
});

app.MapPut("/superhero/{id}", async(DataContext context, SuperHero hero, int id) =>
{
    var dbHero = await context.SuperHeroes.FindAsync(id);

    if (dbHero == null) return Results.NotFound("No Hero Found");

    dbHero.FirstName = hero.FirstName;
    dbHero.LastName = hero.LastName;
    dbHero.HeroName = hero.HeroName;

    await context.SaveChangesAsync();

    return Results.Ok(await GetAllHeroes(context));
});


app.MapDelete("/superhero/{id}", async (DataContext context, int id) =>
{
    var dbHero = await context.SuperHeroes.FindAsync(id);

    if (dbHero == null) return Results.NotFound("Who that?!");

    context.SuperHeroes.Remove(dbHero);
    await context.SaveChangesAsync();

    return Results.Ok(await GetAllHeroes(context));

});
app.Run();  
