 
 
/*
    skapa en recension bok app där man kan lägga till en ny recension. man lägger namn på boken, sin recension, sitt namn och sitt omdöme.  
    The datafile,'book.json', created is in the format of Json.
    det finns en menyval för att välja vad man vill göra
    Written by Najah hawa / Mid Sweden University
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using Microsoft.ML.Transforms.Text;



namespace  SentimentModel.ConsoleApp
{
    public class Booktips
    {
        //här skapas json file
        private string filename = @"book.json";
        //göra klassen Post till lista för att man ska läsa den
        private List<Post> recensioner = new List<Post>();
        
        //metod för att läsa recensioner som finns i jsonfil. 
        public Booktips(){ 
            if(File.Exists(@"book.json")==true){ 
                string? jsonString = File.ReadAllText(filename);
               recensioner = JsonSerializer.Deserialize<List<Post>>(jsonString);
            }
        }
        //metod för att lägga till recension 
        public Post addrecension(Post post){
            recensioner.Add(post);
            marshal();         
            return post;
        }
        //metod för att radera recension
        public int delrecension(int index){
            recensioner.RemoveAt(index);
            marshal();
            return index;
        }

        //metod för att return recensioner som finns lagarade
        public List<Post> getRecensioner(){
            return recensioner;
        }
        //metod för att lägga recensioner i json fil
        private void marshal()
        {
            // Serialize all the objects and save to file
            var jsonString = JsonSerializer.Serialize(recensioner);
            File.WriteAllText(filename, jsonString);
        }

    }

    public class Post
    {
        // här finns propeties som ska används i classen. 
        private string? booknamn;        
        public string? Booknamn
        {
            set {this.booknamn = value;}
            get {return this.booknamn;}
        }
        
        private string? omdome;        
        public string? Omdome
        {
            set {this.omdome = value;}
            get {return this.omdome;}
        }

        private string? recension;        
        public string? Recension
        {
            set {this.recension = value;}
            get {return this.recension;}
        }

          private string? forfattarenamn;        
        public string? Forfattarenamn
        {
            set {this.forfattarenamn = value;}
            get {return this.forfattarenamn;}
        }

           private string? visaresult;        
        public string? Visaresult
        {
            set {this.visaresult = value;}
            get {return this.visaresult;}
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            //vi gör instans av klassssen

            Booktips booktips = new Booktips();
            int i=0;
            //här skriver menyval
            while(true){
                Console.Clear();Console.CursorVisible = false;
                Console.WriteLine("Welcome to Book Tips\n\n");
                Console.WriteLine("1. Add review");
                Console.WriteLine("2. Remove review");
                Console.WriteLine("3. read all reviews");
                Console.WriteLine("4. reviews analys");
                Console.WriteLine("X. Exit\n");
             

              

                int inp = (int) Console.ReadKey(true).Key;
                switch (inp) {
            
                    case '1':
                        Console.Clear();
                        Console.CursorVisible = true; 
                        // vi gör instans av klassen Post
                        Post obj = new Post();
                        // be användare att skriva sitt namn, läser den och spara den i json.
                        Console.Write("Enter your name : ");
                        string? namn = Console.ReadLine();
                        obj.Forfattarenamn = namn;
                        //kontrollera om string är tömt. 
                        if(String.IsNullOrEmpty(namn)) { Console.WriteLine ("is null or empty. Vänligen försök igen" ) ;   }
        
                       
                        Console.Write("Enter the name of the book: ");
                        string? text = Console.ReadLine();
                        obj.Booknamn = text;
                        if(String.IsNullOrEmpty(text)) { Console.WriteLine ("is null or empty. Vänligen försök igen" ) ;   }

                        Console.Write("Enter your review: ");
                        string? yourtext = Console.ReadLine();
                        obj.Recension = yourtext;
                        if(String.IsNullOrEmpty(yourtext)) { Console.WriteLine ("is null or empty. Vänligen försök igen" ) ;   }
        
                        Console.Write("Enter your opinion about the book: ");
                        string? omdome = Console.ReadLine();
                       // om innehåll är mindre än 3 bokstäver sparas inte inlägg 
                        int length = omdome.Length;
                        if (length < 1) { break ;};
                         obj.Omdome = omdome;
                        if(!String.IsNullOrEmpty(text)) booktips.addrecension(obj); 

                      

            // här används machine learning för att analyssera omdöme
            SentimentModel.ModelInput sampleData = new SentimentModel.ModelInput()
            {
              Col0 = omdome
            };

            // Make a single prediction on the sample data and print results
            var predictionResult = SentimentModel.Predict(sampleData);
            //spara resultat i en variable
            var visaresult = (Convert.ToBoolean(predictionResult.PredictedLabel) ? "positiv review" : "negativ review");
            //spara den i json
            obj.Visaresult = visaresult;
            Console.WriteLine("You added new reviw. read it i view all reviews " );
            Console.WriteLine("Your review is :");
            Console.WriteLine(visaresult);
            Console.WriteLine("=============== End of process, hit any key to finish ===============");
            Console.ReadKey();

      
              break;
                    case '2': 
                        Console.Clear();
                        Console.CursorVisible = true;
                        Console.Write("Ange index att radera: ");
                        //visa recensioner i booktips för att visa index som man vill radera
                         i=0;
                        foreach(Post post in booktips.getRecensioner()){
                        Console.WriteLine("[" + i++ + "] " + post.Forfattarenamn + "\n\n "+ post.Booknamn + "\n\n "+ post.Recension + "\n\n " );
                        }
                        //kallar metoden delrecension
                        string? index = Console.ReadLine();
                        booktips.delrecension(Convert.ToInt32(index));

              break;
                    case '3': 
                    
                        Console.Clear();
                        Console.CursorVisible = true;
                        Console.Write(" View all reviews : "+ "\n\n ");
                        //visa alla recensioner i booktips
                         i=0;
                        foreach(Post post in booktips.getRecensioner()){
                        Console.WriteLine("[" + i++ + "] " + " Book name is :  " + post.Booknamn  + "\n\n"+  "Review of book: "+  post.Recension + "\n\n"+  "Book review is  "+  post.Omdome + "\n\n"+  "Analys of review : "+  post.Visaresult + "\n\n"+  "Review writer name: "+  post.Forfattarenamn  + "\n\n");
                        }
                   
                    Console.WriteLine("=============== End of process, hit any key to finish ===============");
                    Console.ReadKey();
                  
               break;
                    case '4': 
                    
                        Console.Clear();
                        Console.CursorVisible = true;
                        Console.Write("List of analys av writen reviews of every book "+ "\n\n ");
                        //visa bäckernas namn och omdöme på en lista
                         i=0;
                        foreach(Post post in booktips.getRecensioner()){
                        Console.WriteLine("[" + i++ + "] " + "Book name is:  " + post.Booknamn  + "\n\n"+ "Book review is : "+  post.Omdome + "\n\n"+  "Analys of review  : " +  post.Visaresult  + "\n\n" +  "Review writer name : " +  post.Forfattarenamn  + "\n\n");
                        }
                   
                    Console.WriteLine("=============== End of process, hit any key to finish ===============");
                    Console.ReadKey();
                  
               break;
  
                   
                }
 
            }

        }
    }
}
 

