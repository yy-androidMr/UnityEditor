package yang.mr.com.testmkotlin.util

import android.os.Environment
import java.io.BufferedReader
import java.io.File
import java.io.InputStream
import java.io.InputStreamReader

/**
 * Created by Administrator on 2017/7/12.
 */
class FileUtil {
    companion object {
        open val sdcardRoot = "${Environment.getExternalStorageDirectory().path!!}/"
        open val gameRoot = "${sdcardRoot}Android/data/com.tencent.tmgp.qqx5/"
        open val configRoot = "${gameRoot}files/assetbundles/config/"
        fun createDir(path: String) {
            val f = File(path)
            if (!f.exists()) {
                val mkR = f.mkdirs()
                System.err.println("mkR:$mkR")
            }
        }


        fun replaceNoFile(file: File, value: String, inputS: InputStream) {
            val br = BufferedReader(InputStreamReader(inputS))
            var line = br.readLine()
            var buffer = StringBuffer()
            while (line != null) {
                line = String.format(line, value)
                buffer.append(line + "\n")
                line = br.readLine()
            }
            file.createNewFile()
            file.writeText(buffer.toString())
        }

        fun replaceHasFile(file: File, startRegix: String, newLine: String) {
            val list: List<String> = file.readText().split("\n")
            var buffer = StringBuffer()
            list.forEach {
                if (it.trim().startsWith(startRegix)) {
                    buffer.append(newLine + "\n")
                } else {
                    buffer.append(it + "\n")
                }
            }
            file.writeText(buffer.toString())
        }
    }
}