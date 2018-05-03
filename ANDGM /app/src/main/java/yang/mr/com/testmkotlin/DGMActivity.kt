package yang.mr.com.testmkotlin

import android.os.Bundle
import android.support.design.widget.BottomNavigationView
import android.support.v7.app.AppCompatActivity
import android.view.MenuItem
import android.view.ViewGroup
import kotlinx.android.synthetic.main.activity_dgm.*
import yang.mr.com.testmkotlin.subtab.SubItemManager

class DGMActivity : AppCompatActivity() {

    private val m_SubItemManager by lazy {
        SubItemManager(content as ViewGroup,this)
    }

    private val mOnNavigationItemSelectedListener = { item: MenuItem ->
        when (item.itemId) {
            R.id.navigation_home -> m_SubItemManager.openView(SubItemManager.SubType.VERSION_LIST)!!
            R.id.navigation_dashboard -> m_SubItemManager.openView(SubItemManager.SubType.CLIENT_CONFIG)!!
//            R.id.navigation_notifications -> {
//            }
            else -> false
        }
    }
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_dgm)
        val navigation = navigation as BottomNavigationView
        navigation.setOnNavigationItemSelectedListener(mOnNavigationItemSelectedListener)
        navigation.selectedItemId = R.id.navigation_home
    }

}
