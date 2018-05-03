package yang.mr.com.testmkotlin.subtab

import android.app.Activity
import android.support.annotation.IdRes
import android.support.annotation.LayoutRes
import android.view.View
import android.view.ViewGroup
import org.jetbrains.anko.find
import org.jetbrains.anko.layoutInflater
import java.io.InputStream

/**
 * Created by Administrator on 2017/7/10.
 */
open class SubItemBase(parent: ViewGroup,act:Activity) {
    protected val m_parent = parent
    protected var content: View? = null
    protected val m_Act =act
    fun openAssets(file: String): InputStream {
        return m_Act.assets.open(file)
    }

    open fun ShowMe(): Boolean {
        getContent()
        if (content == null) {
            return false
        }
        m_parent.removeAllViews()
        m_parent.addView(content)
        OnShow()
        return true
    }

    open fun getContent() {
    }

    open fun OnShow() {

    }


    fun inflate(@LayoutRes layoutRes: Int): View {
        return m_parent.context.layoutInflater.inflate(layoutRes, null)
    }

    fun <T : View> find(@IdRes idRes: Int): T {
        return m_parent.find<View>(idRes) as T
    }

}
