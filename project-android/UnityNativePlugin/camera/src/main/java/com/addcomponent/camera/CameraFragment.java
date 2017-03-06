package com.addcomponent.camera;

import android.app.Fragment;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.net.Uri;
import android.os.Bundle;
import android.os.Environment;
import android.provider.MediaStore;
import android.support.v4.content.FileProvider;
import android.util.Log;

import com.unity3d.player.UnityPlayer;

import java.io.File;
import java.io.IOException;

import static android.app.Activity.RESULT_OK;

/**
 * Created by tomazsaraiva on 05/03/2017.
 */

public class CameraFragment extends Fragment {

    public static final String TAG = "camera_fragment";

    private static final int REQUEST_IMAGE_CAPTURE = 1;

    String _gameObject;
    String _callback;
    String _path;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        setRetainInstance(true);
    }

    public boolean hasCamera() {
        return UnityPlayer.currentActivity.getPackageManager().hasSystemFeature(PackageManager.FEATURE_CAMERA);
    }

    public void takePicture(String gameObject, String filename, String callback) {

        _gameObject = gameObject;
        _callback = callback;

        UnityPlayer.currentActivity.getFragmentManager().beginTransaction().add(this, TAG).commit();

        Intent intent = new Intent(MediaStore.ACTION_IMAGE_CAPTURE);

        if (intent.resolveActivity(UnityPlayer.currentActivity.getPackageManager()) != null) {

            File dir = new File(UnityPlayer.currentActivity.getExternalFilesDir(Environment.DIRECTORY_PICTURES) + "/");

            boolean dirExists = dir.exists() || dir.mkdirs();

            if(dirExists) {

                try {
                    File file = new File(dir.getAbsolutePath() + "/" + filename);

                    boolean fileExists = file.exists() || file.createNewFile();

                    if(fileExists) {

                        _path = file.getAbsolutePath();

                        // get the file uri using the file provider and give write permission
                        Uri photoURI = FileProvider.getUriForFile(UnityPlayer.currentActivity,
                                "com.addcomponent.camera.provider",
                                file);
                        intent.putExtra(MediaStore.EXTRA_OUTPUT, photoURI);
                        intent.addFlags(Intent.FLAG_GRANT_WRITE_URI_PERMISSION);

                        // start the camera activity
                        this.startActivityForResult(intent, REQUEST_IMAGE_CAPTURE);

                    } else {
                        Log.d(TAG, "FAILED file: " + file.getAbsolutePath());
                    }

                } catch (IOException e) {
                    Log.e(TAG, e.getMessage());
                }

            } else {
                Log.d(TAG, "FAILED dir: " + dir.getAbsolutePath());
            }
        }
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);

        if (requestCode == REQUEST_IMAGE_CAPTURE) {
            // if RESULT_OK send the image path to Unity, otherwise send an empty path
            UnityPlayer.UnitySendMessage(_gameObject, _callback, resultCode == RESULT_OK ? _path : "");
        }
    }
}