package yang.mr.com.testmkotlin.subtab

import android.app.Activity
import android.view.ViewGroup
import org.jetbrains.anko.layoutInflater
import yang.mr.com.testmkotlin.R

/**
 * Created by Administrator on 2017/7/10.
 */

class ClientConfigView(parent: ViewGroup, act: Activity) : SubItemBase(parent, act) {
    override fun getContent() {
        if (content == null) {
            content = m_parent.context.layoutInflater.inflate(R.layout.client_config, null)
        }
    }

}