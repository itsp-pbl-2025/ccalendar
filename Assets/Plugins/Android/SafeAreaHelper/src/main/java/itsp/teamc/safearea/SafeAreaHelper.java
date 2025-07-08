package itsp.teamc.safearea;

import android.app.Activity;
import android.os.Build;
import android.view.View;
import android.view.Window;

import androidx.annotation.NonNull;
import androidx.core.view.ViewCompat;
import androidx.core.view.WindowCompat;
import androidx.core.view.WindowInsetsCompat;

public class SafeAreaHelper {

    public static int getNavigationBarHeight(@NonNull Activity activity) {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.UPSIDE_DOWN_CAKE) { // API 34+
            View decorView = activity.getWindow().getDecorView();
            WindowInsetsCompat insets = ViewCompat.getRootWindowInsets(decorView);
    
            if (insets != null) {
                int navBarInset = insets.getInsets(WindowInsetsCompat.Type.navigationBars()).bottom;
                int gestureInset = insets.getInsets(WindowInsetsCompat.Type.systemGestures()).bottom;
    
                if (navBarInset > gestureInset && navBarInset > 0) { // 3 button navigation
                    return navBarInset;
                } else if (gestureInset > 0) { // gesture navigation                    
                    return gestureInset;
                } else { // !?
                    return 0;
                }
            }
        }

        return 0; // fallback
    }
}