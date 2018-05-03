package yang.mr.com.testmkotlin.subtab

import android.app.Activity
import android.view.ViewGroup
import android.widget.Button
import android.widget.TextView
import org.jetbrains.anko.enabled
import org.jetbrains.anko.onClick
import yang.mr.com.testmkotlin.R
import yang.mr.com.testmkotlin.util.FileUtil
import yang.mr.com.testmkotlin.util.StringUtil
import java.io.File
import java.io.FileNotFoundException


/**
 * Created by Administrator on 2017/7/10.
 */

class VersionView(parent: ViewGroup, act: Activity) : SubItemBase(parent, act) {

    val version_listPath = "${FileUtil.configRoot}shared/sdklogin/"
    val version_FileName = "version_list.xml"

    override fun getContent() {
//        val content
        if (content == null) {
            content = inflate(R.layout.version_list)
        }
    }

    override fun OnShow() {
        find<Button>(R.id.refresh).onClick { init() }
        init()
//
    }

    private fun init() {
        val curS = getServerValue("$version_listPath$version_FileName")
        val tagS = getServerValue("${FileUtil.sdcardRoot}$version_FileName")
        find<TextView>(R.id.now_server).text = "现在的服务器:\n" + curS
        find<TextView>(R.id.new_server).text = "要替换的服务器:\n" + tagS

        val rplf = find<Button>(R.id.replaceLocalFile)
        if (curS.equals(tagS)) {
            rplf.enabled = false
            rplf.text = "替换个毛啊.服务器一致的"
        } else {
            rplf.enabled = true
            rplf.text = "开始替换"
        }
        rplf.onClick {
            FileUtil.createDir(version_listPath)
            val target = File("$version_listPath$version_FileName")
            if (!target.exists()) {
                FileUtil.replaceNoFile(target, tagS, openAssets("version_list.xml"))
            } else {
                FileUtil.replaceHasFile(target, "<server ip=\"", "\t\t<server ip=$tagS port=\"33018\"/>")
            }
            init()
        }
    }

    private fun getServerValue(filePath: String): String {
        val file = File(filePath)
        try {
            var content = file.readText()
            val list: List<String> = content.split("\n")
            list.forEach {
                if (it.trim().startsWith("<server ip=\"")) {
                    return StringUtil.getXmlItemAttributeValue(it, "ip")
                }
            }
        } catch (e: FileNotFoundException) {
        }
        return ""
    }


}