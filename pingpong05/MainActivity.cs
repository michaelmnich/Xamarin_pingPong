using System;
using System.IO;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Timers;
using System.Threading.Tasks;
using Android.Content.PM;
using Android.Graphics;
using Android.Media;
using Android.Test.Suitebuilder;
using Android.Util;
using Java.IO;
using pingpong05;
using File = Java.IO.File;
using FileNotFoundException = Java.IO.FileNotFoundException;
using IOException = Java.IO.IOException;
using Stream = System.IO.Stream;
//using Android.Gms.Ads;
namespace PingPong03
{
    [Activity(Label = "PingPong03", MainLauncher = true, Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait)]//, Theme = "@android:style/Theme.NoTitleBar")]

    //[Android(ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : Activity
    {
        private int count = 1;
        private int spead = 3;
        private int xPlus = 3;
        private int yPlus = 3;
        private  int plaerSpead = 4;
        private int points = 0;
        private  bool flagstart = false;
        private  Random rnd = new Random();
        private GridLayout ball;
        private GridLayout bonus01;
        private GridLayout plaer;
        private GridLayout ads;
        private ImageButton button;
        private ImageButton buttonL;
        private ImageButton buttonR;
        private Button buttonStart;
        private ImageButton buttonShare;
        private FrameLayout playfield;
        private TextView lvl;
        private TextView scoreInfo;
        private Task t;
        private float padding = 0;
        private bool goUp = true;
        private int plaerMoveCounter=0;
        private int plaerMouveMode = 30;
        private ImageView img;
        private string buttoner = "start";
        protected override void OnCreate(Bundle bundle)
        {

            base.OnCreate(bundle);
            Window.RequestFeature(WindowFeatures.NoTitle);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            //Ustawienia początkowe------------------------------------------
            button = FindViewById<ImageButton>(Resource.Id.MyButton);
            buttonStart = FindViewById<Button>(Resource.Id.startButton);
            buttonL = FindViewById<ImageButton>(Resource.Id.buttonL);
            buttonR = FindViewById<ImageButton>(Resource.Id.buttonR);
            buttonShare = FindViewById<ImageButton>(Resource.Id.shareButton);
            img = FindViewById< ImageView> (Resource.Drawable.Icon);
            lvl = FindViewById<TextView>(Resource.Id.LVLInfo);
            lvl.SetTextColor(Color.DarkGray);
            lvl.TextSize = 28;
            scoreInfo = FindViewById<TextView>(Resource.Id.ScoreInfo);
            scoreInfo.Text = "score: "+ points;
            ball = FindViewById<GridLayout>(Resource.Id.bal);
            bonus01 = FindViewById<GridLayout>(Resource.Id.bous01);
            ads = FindViewById<GridLayout>(Resource.Id.adsLayout);
            plaer = FindViewById<GridLayout>(Resource.Id.plaer);
            playfield = FindViewById<FrameLayout>(Resource.Id.frameLayout1);
            this.setGame();
            button.Visibility = ViewStates.Invisible;
            ball.Visibility = ViewStates.Invisible;
            plaer.Visibility = ViewStates.Invisible;
            lvl.Visibility = ViewStates.Invisible;
            buttonShare.Visibility = ViewStates.Invisible;
            bonus01.Visibility = ViewStates.Invisible;
            //var bg = button.Background;


            //Ustawienia początkowe------------------------------------------

            //var ad = new AdView(con);
            //ad.AdSize = AdSize.SmartBanner;
            //ad.AdUnitId = 'your id here';
            //var requestbuilder = new AdRequest.Builder();
            //ad.LoadAd(requestbuilder.Build());
            //var layout = FindViewById<LinearLayout>(Resource.Id.mainlayout);
            //layout.AddView(ad);

            buttonStart.Click += delegate
            {
                button.Visibility = ViewStates.Visible;
                buttonStart.Visibility = ViewStates.Invisible;
                ball.Visibility = ViewStates.Visible;
                plaer.Visibility = ViewStates.Visible;
                lvl.Visibility = ViewStates.Visible;
                buttonShare.Visibility = ViewStates.Visible;
                // button.Background = bg;
                if (buttoner == "start")
                { button.SetImageResource(Resource.Drawable.pause);
                    buttoner = "stop";
                }
                else { button.SetImageResource(Resource.Drawable.play);
                    buttoner = "start";
                }
                if (!flagstart)
                {
                    flagstart = true;
                    setGame();
                    retrieveset();
                   t = Run();
                    //ustavic scora na zerowego
                }
                else
                    flagstart = false;
            };

            button.Click += delegate
            {
                if (buttoner == "start")
                {
                    button.SetImageResource(Resource.Drawable.pause);
                    buttoner = "stop";
                }
                else
                {
                    button.SetImageResource(Resource.Drawable.play);
                    buttoner = "start";
                }
                if (!flagstart)
                {
                    flagstart = true;
                    //setGame();
                   t = Run(); 
                    //ustavic scora na zerowego
                }
                else
                    flagstart = false;
            };

            buttonShare.Click += delegate
            {
                this.PostToMyWall();
            };


            buttonL.Touch += delegate
            {
                if (plaer.GetX() > -10)
                {
                    float x = plaer.GetX() - plaerSpead;
                    plaer.SetX(x);
                }
            };

            buttonR.Touch += delegate
            {
                if(plaer.GetX()< (playfield.Width-plaer.Width ))
                {
                    float x = plaer.GetX() + plaerSpead;
                    plaer.SetX(x);
                }
             
            };

        }



        /// <summary>
        ///  obsługa poruszania się piłki.
        /// </summary>
        /// <returns></returns>
        async Task Run()
        {

            
            while (flagstart)
            {
                await Task.Delay(30);
                //warunek odbicia od sciany lewa prawa----------------------------
                if (ball.GetX() >= playfield.Width) { xPlus = -spead; }
                if (ball.GetX() <= 0) { xPlus = spead; }
                //warunek odbicia od sciany lewa prawa----------------------------

                //warunek odbicia od sufitu---------------------------------------
                if (ball.GetY() < 10) { yPlus = spead; }
                //warunek odbicia od sufitu---------------------------------------

                //odbicie od paletki----------------------------------------------
                if (ball.GetX() >= plaer.GetX() &&
                    ball.GetX() <= plaer.GetX()+plaer.Width &&
                   ball.GetY() >= plaer.GetY() &&
                   ball.GetY() <= plaer.GetY() +plaer.Height
                    )
                {
                    yPlus = -spead;
                    points++;
                    scoreInfo.Text = "score: " + points;
                    int ifLvling = rnd.Next(1, 2);
                    if (ifLvling == 1) Leveling();

                   
                }

                if (points > 30)
                {
                    if (points > 40) plaerMouveMode = 50;
                    if (points > 50) plaerMouveMode = 100;
                    if (points > 60) plaerMouveMode = 150;
                    if (points > 70) plaerMouveMode = 200;
                    if (plaerMoveCounter >= plaerMouveMode)
                    {
                        goUp = false;
                    }
                    if (plaerMoveCounter <= 0)
                    {
                        goUp = true;
                    }
                    if (goUp) {
                        padding = plaer.GetY() - 1;
                        plaerMoveCounter++;
                    } else {
                        padding = plaer.GetY() + 1;
                        plaerMoveCounter--;
                    }
                    plaer.SetY(padding);
                }
                if (points > 100)
                {
                    ball.SetBackgroundColor(Color.DarkGray);
                }

                //odbicie od paletki----------------------------------------------

                //Warunek przegranej----------------------------------------------
                if (ball.GetY() > playfield.Height) { this.gameOver(); }
                //Warunek przegranej----------------------------------------------



                float x = ball.GetX() + xPlus;
                ball.SetX(x);
                float y = ball.GetY() + yPlus;
                ball.SetY(y);
            }
           
        }


        private void Leveling()
        {
           
            int speedlvl = rnd.Next(1, 5);
            switch (speedlvl)
            {
                case 1:
                    spead = 6;
                    lvl.Text = "LVL Speed: 2";
                    break;
                case 2:
                    spead = 9;
                    lvl.Text = "LVL Speed: 3";
                    plaerSpead = 6;
                    break;
                case 3:
                    spead = 12;
                    lvl.Text = "LVL Speed: 4";
                    plaerSpead = 8;
                    break;
                case 4:
                    spead = 15;
                    lvl.Text = "LVL Speed: 5";
                    plaerSpead = 9;
                    break;
                case 5:
                    spead = 19;
                    lvl.Text = "LVL Speed: 6";
                    plaerSpead = 9;
                    break;
                default:
                    spead = 3;
                    lvl.Text = "LVL Speed: 1";
                    plaerSpead = 4;
                    break;
            }

        }

        private void gameOver()
        {
            button.Visibility = ViewStates.Invisible;
            ball.Visibility = ViewStates.Invisible;
            plaer.Visibility = ViewStates.Invisible;
           // lvl.Visibility = ViewStates.Invisible;
            buttonStart.Visibility = ViewStates.Visible;
            flagstart = false;
            button.SetImageResource(Resource.Drawable.play);
            // button.SetBackgroundColor(Color.Red);
            lvl.Text = "-= GAME OVER =-";
            var prefs = Application.Context.GetSharedPreferences("MyApp", FileCreationMode.Private);
            prefs.Edit().Clear().Commit();
            t.Dispose();
        }

        private void setGame()
        {
            ball.SetX(playfield.Width / 2);
            ball.SetY(12);
            spead = 3;
            xPlus = spead;
            yPlus = spead;
            plaerSpead = 4;
            points=0;
            lvl.Text = "LVL Speed: 1";
            scoreInfo.Text = "score: " + points;
        }




        public override void OnBackPressed()
        {
            if (lvl.Text != "-= GAME OVER =-")
            {
                saveset();
            }
            else
            {
                var prefs = Application.Context.GetSharedPreferences("MyApp", FileCreationMode.Private);
                prefs.Edit().Clear().Commit();
            }
            flagstart = false;
           // t.Dispose();
            Task.Delay(100);
            base.OnBackPressed();
        }


        private void share()
        {

        }

        protected void saveset()
        {

            //store
           // if(
            var prefs = Application.Context.GetSharedPreferences("MyApp", FileCreationMode.Private);
            var prefEditor = prefs.Edit();
            prefEditor.PutString("ballx", ball.GetX().ToString());
            prefEditor.PutString("bally", ball.GetY().ToString());
            prefEditor.PutString("plaerx", plaer.GetX().ToString());
            prefEditor.PutString("plaery", plaer.GetY().ToString());
            prefEditor.PutString("score", points.ToString());
            prefEditor.PutString("lvl", lvl.Text);
            prefEditor.PutString("speed", spead.ToString());
            prefEditor.PutString("speedX", xPlus.ToString());
            prefEditor.PutString("speedY", yPlus.ToString());
            prefEditor.PutString("plaerSpeed", plaerSpead.ToString());
            prefEditor.PutString("issaved", "save");
            prefEditor.Commit();

        }

        // Function called from OnCreate
        protected void retrieveset()
        {
            //retreive 
            var prefs = Application.Context.GetSharedPreferences("MyApp", FileCreationMode.Private);
            var issaved = prefs.GetString("issaved", null);
            if (issaved== "save")
            {
                var ballx = prefs.GetString("ballx", null);
                ball.SetX(float.Parse(ballx));
                var bally = prefs.GetString("bally", null);
                ball.SetY(float.Parse(bally));

                var savedSpeed = prefs.GetString("speed", null);
                spead = int.Parse(savedSpeed);

                var savedSpeedX = prefs.GetString("speedX", null);
                xPlus = int.Parse(savedSpeedX);

                var savedSpeedY = prefs.GetString("speedY", null);
                yPlus = int.Parse(savedSpeedY);

                var savedScore = prefs.GetString("score", null);
                points = int.Parse(savedScore);
                scoreInfo.Text = "score: " + points;

                var savedLvl = prefs.GetString("lvl", null);
                lvl.Text = savedLvl;

                var plaerx = prefs.GetString("plaerx", null);
                plaer.SetX(float.Parse(plaerx));
                var plaery = prefs.GetString("plaery", null);
                plaer.SetY(float.Parse(plaery));

            }

            //Show a toast
            //RunOnUiThread(() => Toast.MakeText(this, somePref, ToastLength.Long).Show());

        }

        void PostToMyWall()
        {
          //  var uri = Android.Net.Uri.FromFile(GetFileStreamPath("test.jpg"));
            var uri = Android.Net.Uri.Parse("file://" + Android.OS.Environment.ExternalStorageDirectory + "/DCIM/100ANDRO/DSC_0026.jpg");
            //Bitmap bmp =takeScreenshot();
            //bmp.Save()

            Intent sendIntent = new Intent();
            sendIntent.SetAction(Intent.ActionSend);
            sendIntent.PutExtra(Intent.ExtraStream, uri);
            sendIntent.PutExtra(Intent.ExtraText, "http://www.codeofaninja.com");
            sendIntent.PutExtra(Intent.ExtraSubject, "test");

            sendIntent.SetType("text/plain");
            // sendIntent.SetType("image/*");
            // StartActivity(sendIntent);
            // StartActivity(Intent.CreateChooser(sendIntent, "Share Image!"));
            StartActivity(Intent.CreateChooser(sendIntent, "Share link!"));
        }

        public Bitmap takeScreenshot()
        {
            View rootView = FindViewById(Android.Resource.Id.Content).RootView;
            rootView.DrawingCacheEnabled = true;
            return rootView.GetDrawingCache(true);
        }

        public void saveBitmap(Bitmap bitmap)
        {
            //File file = new File(Environment.getExternalStorageDirectory().getAbsolutePath(), "AppName");
            //if (!file.Exists())
            //{
            //    file.Mkdir();
            //}
            //String image_name = "IMG_Name.jpg";
            //File outputFile = new File(file, image_name);
            //FileOutputStream fos = new FileOutputStream(outputFile);
            //fos.Flush();
            //fos.Write(Bytes.toByteArray());
            //fos.Close();

        }

    }
}

