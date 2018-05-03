package yang.mr.com.testmkotlin.util


/**
 * Created by Administrator on 2017/7/14.
 */
open class StringUtil {
    companion object {
        fun getXmlItemValue(item: String) {
            val reg = Regex(">.*<")
            val result = reg.find(item)
            d(result?.value ?: "没找到")
        }

        fun getXmlItemAttributeValue(item: String, attributeName: String): String {
            val numPattern = "[a-zA-Z0-9._]+ *= *\"[^\"\\r\\n]*\"".toRegex()
            for (matchResult in numPattern.findAll(item)) {
                d(matchResult.value)
                val kv = matchResult.value.split('=')
                if (kv[0].trim().equals(attributeName)) {
                    d("getXmlItemAttributeValue:找到${kv[1]}")
                    return kv[1]
                }
            }
            return ""
        }

    }
}