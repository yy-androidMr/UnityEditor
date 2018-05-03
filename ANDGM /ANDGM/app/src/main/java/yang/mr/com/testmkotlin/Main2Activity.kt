package yang.mr.com.testmkotlin

import android.media.AudioManager
import android.media.SoundPool
import android.os.Bundle
import android.support.v7.app.AppCompatActivity
import org.jetbrains.anko.button
import org.jetbrains.anko.onClick
import org.jetbrains.anko.textView
import org.jetbrains.anko.verticalLayout


class Main2Activity : AppCompatActivity() {

    companion object static {
        val ID_TOOLBAR: Int = 1
        val ID_USER_EDIT: Int = 2
        val ID_PSD_EDIT: Int = 3
        val ID_BTN_LOGIN: Int = 4
    }

    var m_soundPoll: SoundPool = SoundPool(32, AudioManager.STREAM_MUSIC, 1)
    var soundList = mutableListOf<String>()
    var soundMap = mutableMapOf<Integer, Integer>()
    var poolIndexOffset = 100
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        var filepath = assets.list("audio/sound_effect")
        for (item in filepath) {
            soundList.add("audio/sound_effect/$item")
        }
        println("count:" + soundList.size)
        verticalLayout {
            val tv = textView("")
            button("init pool") {
                onClick {
                    soundMap.clear()
                    //      m_soundPoll = SoundPool(32, AudioManager.STREAM_MUSIC, 1)
                    for ((index, tem) in soundList.withIndex()) {
                        loadSound(Integer(index + poolIndexOffset), tem)
                    }
                    poolIndexOffset += 100
                    tv.text = "被初始化"
                }
            }
            button("play random pool") {
                onClick {
                    for ((key, value) in soundMap) {
                        println("pool!!:$key,soundid :$value")
                    }
                }
            }
            button("release pool") {
                onClick {
                    for ((key, value) in soundMap) {
                        m_soundPoll.unload(value.toInt())
                    }
                    m_soundPoll.release()
                    tv.text = "已经卸载"
                }
            }
        }

    }

    fun loadSound(id: Integer, path: String) {
        val afd = assets.openFd(path)
        val sound_id = m_soundPoll.load(afd, 1)
        soundMap[id] = Integer(sound_id)
    }
}
