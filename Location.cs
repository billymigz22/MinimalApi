namespace MinimalApi
{
    public class Location
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public SuperHero? SuperHero { get; set; }
        
        public int SuperHeroId { get; set; }

    }
}
