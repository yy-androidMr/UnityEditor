package yang.mr.com.testmkotlin.subtab

import android.app.Activity
import android.view.ViewGroup
import java.io.InputStream
import java.util.*

/**
 * Created by Administrator on 2017/7/10.
 */

class SubItemManager(contentParent: ViewGroup, act: Activity) {

    private var m_ContentParent = contentParent
    private val m_Activity = act

    enum class SubType {
        VERSION_LIST,
        CLIENT_CONFIG,
    }

    val itemList: TreeMap<SubType, SubItemBase?> = TreeMap()
    fun openView(type: SubType): Boolean? {
        if (!itemList.containsKey(type) || (itemList[type] == null)) {
            //创建
            itemList.put(type, createSubItem(m_ContentParent, type))
        }
        return itemList[type]?.ShowMe()
    }


    fun createSubItem(parent: ViewGroup, type: SubType): SubItemBase? {
        when (type) {
            SubType.VERSION_LIST -> {
                return VersionView(parent,m_Activity)
            }
            SubType.CLIENT_CONFIG -> {
                return ClientConfigView(parent,m_Activity)
            }
        }
        return null
    }
}

