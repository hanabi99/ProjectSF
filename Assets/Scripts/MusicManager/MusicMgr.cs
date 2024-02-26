using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MyGameFrameWork
{
    public class MusicMgr : Singleton<MusicMgr>
    {
        //唯一的背景音乐组件
        private AudioSource bkMusic = null;
        //音乐大小
        private float bkValue = 1f;

        //音效依附对象
        private Dictionary<string, GameObject> soundObjDic = new Dictionary<string, GameObject>();

        public List<GameObject> soundObjs = new List<GameObject>();
        //音效列表
        [HideInInspector]
        public List<AudioSource> soundList = new List<AudioSource>();
        //音效大小
        //public float soundValue = 1;

        private void Update()
        {
            for (int i = soundList.Count - 1; i >= 0; --i)
            {
                if (!soundList[i].isPlaying)
                {
                    GameObject.Destroy(soundList[i]);
                    soundList.RemoveAt(i);
                }
            }
        }
        /// <summary>
        /// 播放背景音乐
        /// </summary>
        /// <param name="name"></param>
        public void PlayBkMusic(string name)
        {
            if (bkMusic == null)
            {
                GameObject obj = new GameObject();
                obj.name = "BkMusic";
                bkMusic = obj.AddComponent<AudioSource>();
                soundObjs.Add(obj);
            }
            //异步加载背景音乐 加载完成后 播放
            ResMgr.GetInstance().LoadAsync<AudioClip>("Music/BK/" + name, (clip) =>
            {
                bkMusic.clip = clip;
                bkMusic.loop = true;
                bkMusic.volume = bkValue;
                bkMusic.Play();
            });
        }

        /// <summary>
        /// 暂停背景音乐
        /// </summary>
        public void PauseBKMusic()
        {
            if (bkMusic == null)
                return;
            bkMusic.Pause();
        }

        /// <summary>
        /// 停止背景音乐
        /// </summary>
        public void StopBKMusic()
        {
            if (bkMusic == null)
                return;
            bkMusic.Stop();
        }

        /// <summary>
        /// 改变背景音乐 音量大小
        /// </summary>
        /// <param name="v"></param>
        public void ChangeBKValue(float v)
        {
            bkValue = v;
            if (bkMusic == null)
                return;
            bkMusic.volume = bkValue;
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        public void PlaySound(string resnmae, string objname, bool isLoop, float soundValue, UnityAction<AudioSource> callBack = null)
        {
            if (soundObjDic[objname] == null)
            {
                GameObject obj = new GameObject();
                obj.name = objname;
                soundObjDic.Add(objname, new GameObject());
            }
            //当音效资源异步加载结束后 再添加一个音效
             ResMgr.GetInstance().LoadAsync<AudioClip>("Music/Sound/" + resnmae, (clip) =>
            {
                AudioSource source = soundObjDic[objname].GetComponent<AudioSource>();
                if (source == null)
                {
                    source = soundObjDic[objname].AddComponent<AudioSource>();
                }
                source.clip = clip;
                source.loop = isLoop;
                source.volume = soundValue;
                source.Play();
                if (!soundList.Contains(source))
                {
                    soundList.Add(source);
                }
                if (callBack != null)
                    callBack(source);
            });
        }

        /// <summary>
        /// 改变所有音效声音大小
        /// </summary>
        /// <param name="value"></param>
        public void ChangeSoundValue(float value)
        {
            ChangeBKValue(value);
            for (int i = 0; i < soundList.Count; ++i)
            {
                soundList[i].volume = value;
            }            
        }
        /// <summary>
        /// 改变指定音效声音大小
        /// </summary>
        /// <param name="value"></param>
        public void ChangeOneSoundValue(string sourcename, float value)
        {
            AudioSource source = GetSource(sourcename);
            //soundValue = value;
            source.volume = value;
        }

        /// <summary>
        /// 停止指定音效
        /// </summary>
        public void StopSound(string name)
        {
            AudioSource source = GetSource(name);
            if (soundList.Contains(source))
            {
                source.Stop();
            }
        }
        /// <summary>
        /// 停止所有音效
        /// </summary>
        public void StopAllSound()
        {
            for (int i = 0; i < soundList.Count; i++)
            {
                soundList[i].Stop();
            }
            StopBKMusic();
        }
        /// <summary>
        /// 播放所有音效
        /// </summary>
        public void StartAllSound()
        {
            for (int i = 0; i < soundList.Count; i++)
            {
                soundList[i].Play();
            }
            PlayBkMusic("TaiJiBK");
        }


        /// <summary>
        /// 获取一个音效
        /// </summary>
        public AudioSource GetSource(string sourceObjname)
        {
            for (int i = 0; i < soundList.Count; i++)
            {
                if (soundList[i].gameObject.name == sourceObjname)
                {
                    return soundList[i];
                }
            }
            return null;
        }
    }
}
