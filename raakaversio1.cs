using Jypeli;
using System;
public class raakaversio1 : PhysicsGame
{
    public PlatformCharacter Ukko;
    public Image Pelaaja = LoadImage("C:\\Kurssit\\Peliprojekti\\raakaversio1\\Erikoistehosteet\\Ukkeli.jpg");
    public Image Este1 = LoadImage("C:\\Kurssit\\Peliprojekti\\raakaversio1\\Erikoistehosteet\\Este1.jpg");
    public Image Mimmi = LoadImage("C:\\Kurssit\\Peliprojekti\\raakaversio1\\Erikoistehosteet\\Mimmi.jpg");
    public Image Pohja = LoadImage("C:\\Kurssit\\Peliprojekti\\raakaversio1\\Erikoistehosteet\\ground.png");
    public Image Piikki = LoadImage("C:\\Kurssit\\Peliprojekti\\raakaversio1\\Erikoistehosteet\\karkkikuppijee-removebg-preview.png");
    public Image Taustakuva = LoadImage("C:\\Kurssit\\Peliprojekti\\raakaversio1\\Erikoistehosteet\\background.gif");
    public Image Lipputanko = LoadImage("C:\\Kurssit\\Peliprojekti\\raakaversio1\\Erikoistehosteet\\Lipputanko.gif");


    public IntMeter Vanhusmittari;
    const double Estevauhti = 100;
    const double Juoksuvauhti = 200;
    const double Hyppykorkeus = 100;
    public const int RUUDUN_KOKO = 30;
    public SoundEffect PunainenValo = LoadSoundEffect("C:\\Kurssit\\Peliprojekti\\raakaversio1\\Erikoistehosteet\\PunainenValo.wav");
    public SoundEffect VihreaValo = LoadSoundEffect("C:\\Kurssit\\Peliprojekti\\raakaversio1\\Erikoistehosteet\\VihreaValo.wav");
    public SoundEffect Kuorsaus = LoadSoundEffect("C:\\Kurssit\\Peliprojekti\\raakaversio1\\Erikoistehosteet\\Kuorsaus.wav");
    public override void Begin()
    {
        CenterWindow();
        Gravity = new Vector (0, -1000);
        LuoKentta();
        VanhusMittaristo();
        Timer Kaantyminen = new Timer();
        Kaantyminen.Interval = 3.0;
        Kaantyminen.Start();
        Kaantyminen.Timeout += KaannosArvonta;
        Camera.ZoomFactor = 2;
        Camera.Follow(Ukko);
        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }


    /// <summary>
    /// Aliohjelma arpoo jonkin luvun 1-10. Mikäli luku on pienempi tai
    /// yhtä pieni kuin viisi, ohjelma avaa aliohjelman "Punainen." 
    /// </summary>
    public void KaannosArvonta()
    {
      int Arvonta = RandomGen.NextInt(0, 10);
        if (Arvonta <=5)
        {
            Punainen();
        }
        else
        {
            Timer UusiKaannos = new Timer();
            UusiKaannos.Interval = 3.0;
            UusiKaannos.Start();
        }

    }

    /// <summary>
    /// Aliohjelma, jolloin henkilö ei saa liikkua. 
    /// Jos aliohjelman aikana jokin näppäin on painettuna, peli päättyy kuolemaan.
    /// </summary>
    public void Punainen()
    {
        Label Varoitus = new Label(Level.Width / 5, 30, "Älä liiku!!");
        Varoitus.Y = 200;
        Varoitus.Color = Color.Yellow;
        Varoitus.LifetimeLeft = TimeSpan.FromSeconds(1.0);
        Add(Varoitus);

        PunainenValo.Play();
        if (Keyboard.IsKeyDown(Key.Right) | Keyboard.IsKeyDown(Key.Left) | Keyboard.IsKeyDown(Key.Space))
        {
            Kuolema();
        }
    }


    /// <summary>
    /// Aliohjelma käynnistyy pelaajan kuollessa.
    /// </summary>
    public void Kuolema()
    {
        Pause();
        MultiSelectWindow lopetus = new MultiSelectWindow("Kuolit :(", "Uusi peli", "Lopeta peli.");
        Add(lopetus);
        lopetus.AddItemHandler(0, Alkuun);
        lopetus.AddItemHandler(1, Exit);

    }

    /// <summary>
    /// Luo kentän tekstitiedoston pohjalta.
    /// </summary>
    public void LuoKentta()
    {
        TileMap kentta = TileMap.FromLevelAsset("C:\\Kurssit\\Peliprojekti\\raakaversio1\\Erikoistehosteet\\kokeiluuaa.txt");
        kentta.SetTileMethod('#', LisaaTaso);
        kentta.SetTileMethod('B', Hahmo);
        kentta.SetTileMethod('Y', Piikit);
        kentta.SetTileMethod('X', Esteet);
        kentta.SetTileMethod('M', Maali);


        kentta.Execute(RUUDUN_KOKO, RUUDUN_KOKO);
        //Level.CreateBorders();
    }

    /// <summary>
    /// Lipun näköinen asia, johon osumalla voittaa pelin.
    /// </summary>
    /// <param name="paikka">Määrätty etukäteen tekstitiedostossa.</param>
    /// <param name="leveys">Määrätty etukäteen tekstitiedostossa.</param>
    /// <param name="korkeus">Määrätty etukäteen tekstitiedostossa.</param>
    public void Maali(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject Lippu = PhysicsObject.CreateStaticObject(leveys, korkeus);
        Lippu.Position = paikka;
        Lippu.Image = Lipputanko;
        Lippu.Tag = "Libbu";
        Add(Lippu);
    }


    /// <summary>
    /// Kentän alla olevat piikit.
    /// </summary>
    /// <param name="paikka">Määrätty etukäteen tekstitiedostossa.</param>
    /// <param name="leveys">Määrätty etukäteen tekstitiedostossa.</param>
    /// <param name="korkeus">Määrätty etukäteen tekstitiedostossa.</param>
    public void Piikit(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject taso = PhysicsObject.CreateStaticObject(leveys, korkeus);
        taso.Position = paikka;
        taso.Tag = "Piikki";
        taso.Image = Piikki;
        Add(taso);
    }

    /// <summary>
    /// Tapahtumasarja henkilön pudotessa kentän alla oleviin piikkeihin.
    /// </summary>
    /// <param name="Hahmo">Pelattava hahmo</param>
    /// <param name="Piikit">Alapuolella olevat piikit</param>
    public void AlaKuolema(PhysicsObject Hahmo, PhysicsObject Piikit)
    {
        Hahmo.Destroy();
        Pause();
        MultiSelectWindow lopetus = new MultiSelectWindow("Kuolit :(", "Uusi peli", "Lopeta");
        Add(lopetus);
        lopetus.AddItemHandler(0, Alkuun);
        lopetus.AddItemHandler(1, Exit);

    }

    /// <summary>
    /// Mikäli pelaaja haluaa käynnistää pelin uudestaan, ohjelma käynnistyy.
    /// </summary>
    public void Alkuun()
    {
        ClearAll();
        Begin();
    }
    /// <summary>
    /// Luo tekstitiedostossa mainitulle paikalle tason.
    /// </summary>
    /// <param name="paikka">Määrätty etukäteen tekstitiedostossa.</param>
    /// <param name="leveys">Määrätty etukäteen tekstitiedostossa.</param>
    /// <param name="korkeus">Määrätty etukäteen tekstitiedostossa.</param>
    public void LisaaTaso(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject taso = PhysicsObject.CreateStaticObject(leveys, korkeus);
        taso.Position = paikka;
        taso.Color = Color.Green;
        taso.Image = Pohja;
        Add(taso);
    }

    /// <summary>
    /// Luo esteen, joka näyttää nukkuvalta vanhukselta.
    /// </summary>
    /// <param name="paikka">Määrätty etukäteen tekstitiedostossa.</param>
    /// <param name="leveys">Määrätty etukäteen tekstitiedostossa.</param>
    /// <param name="korkeus">Määrätty etukäteen tekstitiedostossa.</param>
    public void Esteet(Vector paikka, double leveys, double korkeus)
    {
            PhysicsObject Este = new PhysicsObject(40, 40);
            Este.Mass = 4.0;
        Este.Position = paikka;
            Este.Image = Este1;
            Este.Tag = "Nukkuva vanhus";
            Add(Este);
    }

    /// <summary>
    /// Pelattavan hahmon koodit, törmäystutkat ja näppäimistön tunnisteet.
    /// </summary>
    /// <param name="paikka">Määrätty etukäteen tekstitiedostossa.</param>
    /// <param name="leveys">Määrätty etukäteen tekstitiedostossa.</param>
    /// <param name="korkeus">Määrätty etukäteen tekstitiedostossa.</param>
    public void Hahmo( Vector paikka, double leveys, double korkeus)
    {
        Ukko = new PlatformCharacter(leveys, korkeus);
        Ukko.Position = paikka;
        Ukko.Mass = 4.0;
        Ukko.Image = Pelaaja;
        AddCollisionHandler(Ukko, "Nukkuva vanhus", OsuVanhukseen);
        AddCollisionHandler(Ukko, "Piikki", AlaKuolema);
        AddCollisionHandler(Ukko, "Libbu", Voitto);

        Add(Ukko);
        Keyboard.Listen(Key.Right, ButtonState.Down, Juokse, "Anna mennä.", Ukko, Juoksuvauhti);
        Keyboard.Listen(Key.Left, ButtonState.Down, Juokse, "Mene taaksepäin.", Ukko, -Juoksuvauhti);
        Keyboard.Listen(Key.Space, ButtonState.Down, Hyppaa, "Hyppää ylöspäin.", Ukko, Hyppykorkeus);
    }

    /// <summary>
    /// Pelaajan saavuttaessa maalin, aliohjelma alkaa.
    /// </summary>
    /// <param name="Hahmo">Pelattava henkilö</param>
    /// <param name="Piikit">Voittolippu</param>
    public void Voitto(PhysicsObject Hahmo, PhysicsObject Piikit)
    {
        VihreaValo.Play();
        Pause();
        MultiSelectWindow lopetus = new MultiSelectWindow("Voitit pelin :)", "Uusi peli", "Lopeta peli.");
        Add(lopetus);
        lopetus.AddItemHandler(0, Alkuun);
        lopetus.AddItemHandler(1, Exit);
    }

    /// <summary>
    /// Luo mittarin esteeseen osumiselle.
    /// </summary>
    public void VanhusMittaristo()
    {
        Vanhusmittari = new IntMeter(0);
        Label Mittaristo = new Label();
        Mittaristo.Title = "Herätetyt vanhukset: ";
        Mittaristo.X = Screen.Right - 120;
        Mittaristo.Y = Screen.Top - 100;
        Mittaristo.Color = Color.Yellow;
        Mittaristo.TextColor = Color.Black;
        Mittaristo.BindTo(Vanhusmittari);
        Add(Mittaristo);
    }

    /// <summary>
    /// Aliohjelma, joka käynnistyy esteeseen osumisesta.
    /// Mikäli esteeseen osutaan kolme kertaa, peli päättyy.
    /// </summary>
    /// <param name="Hahmo"></param>
    /// <param name="Este"></param>
    public void OsuVanhukseen(PhysicsObject Hahmo, PhysicsObject Este)
    {
        Vanhusmittari.Value += 1;

        if (Vanhusmittari.Value == 3)
        {
            Kuolema();
        }

        Label Varoitus = new Label(Level.Width / 5, 30, "Surkimus herätit vanhukseen!!");
        Varoitus.Y = 300;
        Varoitus.Color = Color.Yellow;
        Varoitus.LifetimeLeft = TimeSpan.FromSeconds(3.0);
        Add(Varoitus);
        Kuorsaus.Play();
    }


    public void Juokse(PlatformCharacter Ukko, double suunta)
    {
        Ukko.Walk(suunta);
    }

    public void Hyppaa(PlatformCharacter Ukko, double voima)
    {
        Ukko.Jump(voima);
    }


}

