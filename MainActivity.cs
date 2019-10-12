using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Android.Media;
using Android;
using Android.Support.V4.App;
using Android.Content.PM;
using Android.Util;

namespace CaitlinAudio
{
    [Activity(Label = "app_name", MainLauncher = true, Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class MainActivity : AppCompatActivity
    {

        Button btnRecord, btnStopRecord, btnStart, btnPlay, btnStopPlay;
        string pathSave = "";
        MediaRecorder mediaRecorder;
        MediaPlayer mediaPlayer;

        private const int REQUEST_PERMISSION_CODE = 1000;

        private bool isGrantedPermission = false;

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            switch (requestCode)
            {
                case REQUEST_PERMISSION_CODE:
                    {
                        if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                        {
                            Toast.MakeText(this, "Granted", ToastLength.Short).Show();
                            isGrantedPermission = true;
                        }
                        else
                        {
                            Toast.MakeText(this, "Granted", ToastLength.Short).Show();
                        }
                    }
                    break;
            }
        }



        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            if(CheckSelfPermission(Manifest.Permission.WriteExternalStorage) != Android.Content.PM.Permission.Granted
                && CheckSelfPermission(Manifest.Permission.RecordAudio) != Android.Content.PM.Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new string[] {
                        Manifest.Permission.WriteExternalStorage,
                        Manifest.Permission.RecordAudio
                }, REQUEST_PERMISSION_CODE);
            }
            else
            {
                isGrantedPermission = true;
            }
            
            btnRecord = FindViewById<Button>(Resource.Id.btnRecord);
            btnStopRecord = FindViewById<Button>(Resource.Id.btnStopRecord);
            btnPlay = FindViewById<Button>(Resource.Id.btnPlay);
            btnStopPlay = FindViewById<Button>(Resource.Id.btnStopPlay);

            btnStopRecord.Enabled = false;
            btnPlay.Enabled = false;
            btnStopPlay.Enabled = false;


            btnRecord.Click += delegate {
                RecordAudio();
	        };

            btnStopRecord.Click += delegate
            {
                StopRecorder();
            };

            btnPlay.Click += delegate {
                StartLastRecord();
        	};

            btnStopRecord.Click += delegate
            {
                StopLastRecord();
            };
        }


        private void StopLastRecord()
        {
            btnStopPlay.Enabled = false;
            btnStopRecord.Enabled = false;
            btnPlay.Enabled = true;
            btnRecord.Enabled = true;

            if(mediaPlayer != null)
            {
                mediaPlayer.Stop();
                mediaPlayer.Release();
                SetUpMediaRecorder();
            }
        }

        private void StartLastRecord()
        {
            btnStopRecord.Enabled = false;
            btnStopPlay.Enabled = true;
            btnRecord.Enabled = false;
            mediaPlayer = new MediaPlayer();

            try
            {
                mediaPlayer.SetDataSource(pathSave);
                mediaPlayer.Prepare();
            }
            catch(Exception ex)
            {
                Log.Debug("DEBUG", ex.Message);
            }

            mediaPlayer.Start();
            Toast.MakeText(this, "Playing Music", ToastLength.Short).Show();
        }


        private void StopRecorder()
        {
            mediaRecorder.Stop();
            btnRecord.Enabled = true;
            btnStopRecord.Enabled = false;
            btnStopPlay.Enabled = false;
            btnPlay.Enabled = true;

            Toast.MakeText(this, "Stop Recording...", ToastLength.Short).Show();
        }



        private void RecordAudio()
        {
            if (isGrantedPermission)
            {
                pathSave = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath.ToString() + "/" + new Guid().ToString() + "_audio.3gp";
                SetUpMediaRecorder();
                try
                {
                    mediaRecorder.Prepare();
                    mediaRecorder.Start();

                    btnRecord.Enabled = false;
                    btnStopRecord.Enabled = true;

                }
                catch(Exception ex)
                {
                    Log.Debug("DEBUG", ex.Message);
                }
                Toast.MakeText(this, "Recording...", ToastLength.Short).Show();
            }
        }

        private void SetUpMediaRecorder()
        {
            mediaRecorder = new MediaRecorder();
            mediaRecorder.SetAudioSource(AudioSource.Mic);
            mediaRecorder.SetOutputFormat(OutputFormat.ThreeGpp);
            mediaRecorder.SetAudioEncoder(AudioEncoder.AmrNb);
            mediaRecorder.SetOutputFile(pathSave);
        }
	}
}

