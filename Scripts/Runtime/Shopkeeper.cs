using System;
using System.Collections;
using System.Collections.Generic;
using Meta.WitAi.Attributes;
using Meta.WitAi.TTS.Utilities;
using UnityEngine;

namespace DoubTech.Synty.Lipsync.Samples
{
    public class Shopkeeper : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private TTSSpeaker _speaker;

        [SerializeField] private VoiceScript[] _scriptLines;

        private static readonly int Wave = Animator.StringToHash("Wave");

        private Queue<VoiceScript> _scriptQueue = new Queue<VoiceScript>();
        private static readonly int Talking = Animator.StringToHash("Talking");

        private void Start()
        {
            ResetScript();
        }

        private void OnEnable()
        {
            _speaker.Events.OnFinishedSpeaking.AddListener(OnFinishedSpeaking);
            _speaker.Events.OnStartSpeaking.AddListener(OnStartSpeaking);
        }

        private void OnDisable()
        {
            _speaker.Events.OnFinishedSpeaking.RemoveListener(OnFinishedSpeaking);
            _speaker.Events.OnStartSpeaking.RemoveListener(OnStartSpeaking);
        }

        private void OnStartSpeaking(TTSSpeaker arg0, string arg1)
        {
            _animator.SetBool(Talking, true);
        }

        private void OnFinishedSpeaking(TTSSpeaker arg0, string arg1)
        {
            _animator.SetBool(Talking, false);
            StartCoroutine(NextScript());
        }

        private void OnTriggerEnter(Collider other)
        {
            GreetCustomer();
        }

        [Button]
        public void GreetCustomer()
        {
            ResetScript();
            _animator.SetTrigger(Wave);
            StopAllCoroutines();
            StartCoroutine(NextScript());
        }

        private IEnumerator NextScript()
        {
            if (_scriptQueue.Count > 0)
            {
                var line = _scriptQueue.Dequeue();
                Debug.Log("Starting next line: " + line.line);
                yield return new WaitForSeconds(line.delay);
                _speaker.Speak(line.line);
            }
        }

        private void ResetScript()
        {
            _scriptQueue.Clear();
            foreach (var line in _scriptLines)
            {
                _scriptQueue.Enqueue(line);
            }
        }
    }

    [Serializable]
    public class VoiceScript
    {
        [SerializeField] public string line;
        [SerializeField] public float delay;
    }
}
