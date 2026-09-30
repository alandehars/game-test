using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace OuterRimPixel
{
    internal sealed class GameForm : Form
    {
        const int W = 480, H = 270;
        readonly Bitmap frame = new Bitmap(W, H);
        readonly Timer timer = new Timer();
        readonly Random rng = new Random();
        int hp = 12, credits = 430, heat = 1, xp = 0, ticks = 0;
        int scene = 0;
        bool clue = false, berth = false, debtPaid = false, dead = false, ended = false;
        string story = "Docking Ring 94. Venn says the Empire is using relief convoys to bait a Rebel cell. Your ship is impounded, and the collector wants 300 credits.";
        string roll = "";
        readonly string[] options = {
            "Slip past customs with a forged manifest  [DECEPTION +6]",
            "Demand Venn's convoy ledger  [INSIGHT +1]",
            "Bet 50 credits at the sabacc table  [GAMBLING +4]",
            "Slice the dock terminal  [SLICING +2]",
            "Shoot the sprinkler valve  [RANGED +4]"
        };
        Rectangle[] buttons = new Rectangle[5];
        readonly Font font = new Font("Consolas", 9, FontStyle.Bold);
        readonly Font small = new Font("Consolas", 7, FontStyle.Bold);

        public GameForm()
        {
            Text = "OUTER RIM RUN  |  SABRINA'S STORY";
            ClientSize = new Size(960, 540);
            MinimumSize = new Size(976, 579);
            MaximumSize = new Size(976, 579);
            BackColor = Color.FromArgb(8, 10, 22);
            DoubleBuffered = true;
            KeyPreview = true;
            timer.Interval = 90;
            timer.Tick += delegate { ticks++; Invalidate(); };
            timer.Start();
            Paint += DrawGame;
            KeyDown += OnKeyDown;
            MouseDown += OnMouseDown;
        }

        void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode >= Keys.D1 && e.KeyCode <= Keys.D5) Choose(e.KeyCode - Keys.D1);
            else if (e.KeyCode >= Keys.NumPad1 && e.KeyCode <= Keys.NumPad5) Choose(e.KeyCode - Keys.NumPad1);
            else if (e.KeyCode == Keys.S) { story = "SABRINA | Human smuggler, level 1 | HP " + hp + "/12 | Defense 14 | XP " + xp + " | Credits " + credits + " | Heat " + heat + ". Gear: blaster pistol, holdout derringer, marked sabacc deck. Lucky Comet: " + (berth ? "released" : "impounded") + ". Debt: " + (debtPaid ? "paid" : "300 credits owed") + "."; Invalidate(); }
            else if (e.KeyCode == Keys.R && (dead || ended)) ResetGame();
        }
        void OnMouseDown(object sender, MouseEventArgs e)
        {
            float sx = (float)W / ClientSize.Width, sy = (float)H / ClientSize.Height;
            int x = (int)(e.X * sx), y = (int)(e.Y * sy);
            for (int i = 0; i < buttons.Length; i++) if (buttons[i].Contains(x, y)) { Choose(i); return; }
        }
        void ResetGame()
        {
            hp=12; credits=430; heat=1; xp=0; scene=0; clue=false; berth=false; debtPaid=false; dead=false; ended=false;
            story="Docking Ring 94. Venn says the Empire is using relief convoys to bait a Rebel cell. Your ship is impounded, and the collector wants 300 credits."; roll=""; Invalidate();
        }
        int Check(int bonus, int target)
        {
            int d = rng.Next(1,21), total = d + bonus;
            roll = "D20 " + d + " + " + bonus + " = " + total + "  vs  " + target;
            return d == 20 || (d != 1 && total >= target) ? 1 : 0;
        }
        void Choose(int n)
        {
            if (n < 0 || n > 4 || dead || ended) { if (dead || ended) { scene = 1; story="The Lucky Comet climbs into the night above Nar Shaddaa. The Empire's net is still out there, and the Rebel cell needs a warning."; ended=false; } Invalidate(); return; }
            if (scene == 1) { ResolveEscape(n); Invalidate(); return; }
            switch(n)
            {
                case 0:
                    if(Check(6,14)==1) { berth=true; xp+=10; story="The forged manifest passes inspection. A tired clerk releases the Lucky Comet's clamps. The collector shouts something about interest."; }
                    else { heat++; hp--; story="The ink runs in the acid rain. A stun bolt grazes Sabrina as customs seals the dock lane."; }
                    break;
                case 1:
                    if(Check(1,12)==1) { clue=true; xp+=15; story="Venn hands over a convoy ledger and a warning: the relief route is an Imperial ambush. 'I did not sell this one,' he says. 'Probably.'"; }
                    else { heat++; clue=true; story="Venn bolts. A data wafer skitters under a crate: convoy coordinates and an Imperial trap. The collector spots you."; }
                    break;
                case 2:
                    if(credits<50) { story="The pockets are empty. The sabacc table offers no credit."; break; }
                    credits-=50;
                    if(Check(4,15)==1) { credits+=100; xp+=10; if(credits>=300) { credits-=300; debtPaid=true; } story=debtPaid?"A perfect hand. You pay the collector in full; he accepts the credits with the expression of a man losing an argument to a card.":"You win the hand, but still owe the collector. He promises to remember."; }
                    else { heat++; story="The house wins. The collector laughs, and the little sensible voice in your head loses another hand."; }
                    break;
                case 3:
                    if(Check(2,13)==1) { clue=true; berth=true; xp+=20; story="The terminal coughs up an impound release code and a cached convoy ledger. Venn was telling the truth: the Empire is laying a trap for Rebels."; }
                    else { heat++; hp--; story="The terminal locks you out and pings security. A guard's stun baton catches your ribs."; }
                    break;
                case 4:
                    if(Check(4,16)==1) { clue=true; xp+=15; story="The valve bursts. Steam veils the gantry; Venn tosses you the ledger. 'Subtle,' he says. 'Like a falling moon.'"; }
                    else { hp-=3; heat+=2; story="The bolt ricochets. The sprinkler stays dry; the blaster team does not. Sabrina dives under the gantry as fire chews through the railing."; }
                    break;
            }
            if(hp<=0) { hp=0; dead=true; story="Sabrina falls beneath the docking gantry. The Empire takes the ship; Nar Shaddaa keeps the rest of the story."; }
            else if(clue && berth) { scene=1; story="The ledger is real, the ship is free, and Imperial patrols are closing the ring. A Rebel courier pings the Lucky Comet: 'Can you get this warning through?';"; }
            Invalidate();
        }
        void ResolveEscape(int n)
        {
            if(n==0) { if(Check(5,15)==1) { xp+=20; ended=true; story="The Lucky Comet skims between two patrol craft and punches to hyperspace. The ledger reaches the Rebel cell. Somewhere, a convoy changes course—and lives."; } else { hp-=3; heat+=2; story="A patrol interceptor clips the Comet's stabilizer. Warning lights bloom across the cockpit; the Rebel courier repeats, 'We are running out of time.'"; } }
            else if(n==1) { clue=true; xp+=10; story="You transmit the convoy coordinates on a narrow beam. A Rebel voice answers: 'Received. We will warn the transports.' Imperial sensors swing toward your signal."; heat++; }
            else if(n==2) { if(Check(6,14)==1) { ended=true; credits+=500; story="You sell the ledger to a nervous middleman, then slip away with 500 credits. The Rebels may never hear about the convoy. Easy money has a long shadow."; } else { heat++; story="The buyer is an undercover Imperial agent. He reaches for his blaster; the ship's engines are still warm."; hp--; } }
            else if(n==3) { if(credits>=50) { credits-=50; if(Check(4,15)==1) { credits+=100; story="One more hand, one more win. The stars outside look almost like a good omen."; } else { story="The cards turn cold. The patrol beacon turns red. Some habits charge interest."; heat++; } } else story="You pat your jacket. No stake, no game."; }
            else { if(Check(4,16)==1) { ended=true; xp+=25; story="You kick the throttle, skim the patrol's sensor dish, and drop the ledger into the Rebel courier's hands on the way past. The Empire gets rain, sparks, and an empty sky."; } else { hp-=4; heat+=2; story="The throttle sticks. The interceptor's warning shot tears through the cargo bay. The Rebel courier is still waiting."; } }
            if(hp<=0) { hp=0; dead=true; ended=false; story="The Lucky Comet spins out beneath the patrol lights. Sabrina's story ends in the cold dark above Nar Shaddaa."; }
            Invalidate();
        }

        void DrawGame(object sender, PaintEventArgs e)
        {
            using(Graphics g=Graphics.FromImage(frame))
            {
                g.Clear(Color.FromArgb(9,13,29)); g.TextRenderingHint=TextRenderingHint.SingleBitPerPixelGridFit;
                if(scene==0) DrawDock(g); else DrawSpace(g);
                DrawPanel(g);
            }
            e.Graphics.InterpolationMode=InterpolationMode.NearestNeighbor; e.Graphics.PixelOffsetMode=PixelOffsetMode.Half;
            e.Graphics.DrawImage(frame,new Rectangle(0,0,ClientSize.Width,ClientSize.Height),0,0,W,H,GraphicsUnit.Pixel);
        }
        void Box(Graphics g,int x,int y,int w,int h,Color c) { using(Brush b=new SolidBrush(c)) g.FillRectangle(b,x,y,w,h); }
        void Txt(Graphics g,string s,int x,int y,Color c) { using(Brush b=new SolidBrush(c)) g.DrawString(s,font,b,x,y); }
        void DrawDock(Graphics g)
        {
            Box(g,0,0,W,143,Color.FromArgb(12,19,42));
            // distant neon skyline
            for(int i=0;i<20;i++){int x=i*26-(ticks%26); int h=24+(i*37%55); Box(g,x,84-h,19,h,Color.FromArgb(23,30,62)); if(i%3==0) Box(g,x+5,94-h,3,3,Color.FromArgb(30,198,217));}
            Box(g,18,24,94,18,Color.FromArgb(31,14,57)); Txt(g,"DOCK 94",25,26,Color.FromArgb(247,74,199));
            Box(g,355,30,104,20,Color.FromArgb(29,13,49)); Txt(g,"NAR SHADDAA",359,33,Color.FromArgb(53,228,222));
            // gantry and landing pad
            Box(g,0,111,W,7,Color.FromArgb(62,72,101)); Box(g,28,75,5,48,Color.FromArgb(76,86,115)); Box(g,30,75,158,4,Color.FromArgb(90,99,127)); Box(g,322,67,5,59,Color.FromArgb(76,86,115)); Box(g,322,67,116,4,Color.FromArgb(90,99,127));
            // ship in clamp cradle
            Box(g,302,92,104,13,Color.FromArgb(99,110,131)); Box(g,324,83,53,12,Color.FromArgb(160,165,174)); Box(g,337,76,32,9,Color.FromArgb(176,179,183)); Box(g,370,86,41,9,Color.FromArgb(132,140,158)); Box(g,316,101,5,13,Color.FromArgb(241,69,137)); Box(g,394,101,5,13,Color.FromArgb(241,69,137));
            // Venn, Sabrina, collector, customs trooper
            Sprite(g,118,91,Color.FromArgb(32,204,196),false); Sprite(g,204,90,Color.FromArgb(242,155,54),false); Sprite(g,257,92,Color.FromArgb(229,73,185),false); Sprite(g,438,92,Color.FromArgb(165,179,209),true);
            // animated acid rain
            for(int i=0;i<42;i++){int x=(i*71+ticks*3)%W, y=(i*43+ticks*5)%106; Box(g,x,y,1,4,Color.FromArgb(90,79,128,190));}
            Box(g,0,117,W,25,Color.FromArgb(10,14,29)); Box(g,0,117,W,2,Color.FromArgb(31,220,217));
            Txt(g,"SABRINA",192,73,Color.FromArgb(255,215,111)); Txt(g,"VENN",105,75,Color.FromArgb(72,234,218));
            if((ticks/5)%2==0) Box(g,386,91,3,3,Color.FromArgb(255,62,70));
        }
        void Sprite(Graphics g,int x,int y,Color coat,bool helmet)
        {
            Box(g,x+4,y,8,8,Color.FromArgb(206,151,112));
            Box(g,x+3,y-2,10,4,helmet?Color.FromArgb(200,207,218):Color.FromArgb(51,36,43));
            Box(g,x+2,y+8,12,13,coat); Box(g,x,y+10,3,9,Color.FromArgb(191,141,97)); Box(g,x+14,y+10,3,8,Color.FromArgb(191,141,97));
            Box(g,x+3,y+21,4,7,Color.FromArgb(36,45,72)); Box(g,x+10,y+21,4,7,Color.FromArgb(36,45,72));
            Box(g,x+4,y+3,2,2,Color.FromArgb(20,24,31));
        }
        void DrawSpace(Graphics g)
        {
            Box(g,0,0,W,143,Color.FromArgb(5,9,24));
            for(int i=0;i<90;i++){int x=(i*67)%W,y=(i*37)%137; Box(g,x,y,1+(i%9==0?1:0),1,Color.FromArgb(160,190,220));}
            // moon and city glow
            Box(g,38,22,49,49,Color.FromArgb(60,64,100)); Box(g,46,13,34,9,Color.FromArgb(60,64,100)); Box(g,28,34,10,27,Color.FromArgb(60,64,100)); Box(g,48,31,5,4,Color.FromArgb(242,78,188)); Box(g,69,47,6,4,Color.FromArgb(38,215,219));
            // Lucky Comet
            Box(g,168,68,130,17,Color.FromArgb(157,170,190)); Box(g,207,56,48,13,Color.FromArgb(187,193,201)); Box(g,285,62,42,11,Color.FromArgb(128,142,167)); Box(g,151,72,19,8,Color.FromArgb(119,134,157)); Box(g,185,85,82,3,Color.FromArgb(86,99,124));
            Box(g,141,72,9,8,Color.FromArgb(46,213,241)); Box(g,329,66,34,4,Color.FromArgb(255,64,101));
            // pursuing patrol
            Box(g,367,38,60,11,Color.FromArgb(158,164,181)); Box(g,383,29,26,9,Color.FromArgb(180,184,194)); Box(g,424,35,29,4,Color.FromArgb(107,118,147)); Box(g,357,41,12,4,Color.FromArgb(255,56,88));
            Sprite(g,226,57,Color.FromArgb(238,158,55),false);
            Box(g,0,111,W,31,Color.FromArgb(12,15,30)); Box(g,0,111,W,2,Color.FromArgb(47,206,214));
            Txt(g,"IMPERIAL PATROL",12,117,Color.FromArgb(255,95,115)); Txt(g,"HYPERDRIVE: STANDBY",315,117,Color.FromArgb(255,205,99));
        }
        string[] Wrap(string text,int max)
        {
            string[] words=text.Split(' '); System.Collections.Generic.List<string> lines=new System.Collections.Generic.List<string>(); string line="";
            foreach(string word in words){if((line+" "+word).Trim().Length>max){lines.Add(line);line=word;}else line=(line+" "+word).Trim();} if(line.Length>0)lines.Add(line); return lines.ToArray();
        }
        void DrawPanel(Graphics g)
        {
            Box(g,0,143,W,127,Color.FromArgb(12,16,31)); Box(g,0,143,W,2,Color.FromArgb(41,226,214));
            Box(g,8,149,464,17,Color.FromArgb(27,33,54)); Txt(g,"HP "+hp+"/12   CRED "+credits+"   HEAT "+heat+"   XP "+xp,14,151,Color.FromArgb(247,207,120));
            int yy=170; foreach(string line in Wrap(story,66)){Txt(g,line,11,yy,Color.FromArgb(224,232,245));yy+=12;if(yy>202)break;}
            if(roll.Length>0) Txt(g,roll,313,150,Color.FromArgb(85,227,216));
            if(dead||ended){Txt(g,dead?"GAME OVER  |  Press R to restart":"CHAPTER COMPLETE  |  Press any key to continue",12,205,Color.FromArgb(255,107,132));return;}
            if(scene==1){string[] opts={"[1] RUN THE PATROL  [PILOTING +5]","[2] WARN THE REBELS  [RISK: HEAT]","[3] SELL THE LEDGER  [DECEPTION +6]","[4] SABACC, ONE LAST HAND  [GAMBLE]","[5] IMPROVISE A DANGEROUS ESCAPE"}; DrawOptions(g,opts,214);}
            else {string[] opts=new string[5]; for(int i=0;i<5;i++) opts[i]="["+(i+1)+"] "+options[i]; DrawOptions(g,opts,214);}
            Txt(g,"CLICK AN ACTION OR PRESS 1-5     S: SHEET",13,258,Color.FromArgb(101,133,166));
        }
        void DrawOptions(Graphics g,string[] opts,int top)
        {
            for(int i=0;i<5;i++){int y=top+i*9; buttons[i]=new Rectangle(8,y-1,464,9); Box(g,8,y-1,464,9,(i%2==0)?Color.FromArgb(20,27,44):Color.FromArgb(15,22,37)); Txt(g,opts[i],12,y,Color.FromArgb(124,225,216));}
        }
    }
    internal static class Program
    {
        [STAThread] static void Main(){Application.EnableVisualStyles();Application.SetCompatibleTextRenderingDefault(false);Application.Run(new GameForm());}
    }
}

