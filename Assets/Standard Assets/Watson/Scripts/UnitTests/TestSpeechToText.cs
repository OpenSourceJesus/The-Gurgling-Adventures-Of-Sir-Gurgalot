﻿/**
* Copyright 2015 IBM Corp. All Rights Reserved.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/


using FullSerializer;
using IBM.Watson.DeveloperCloud.Connection;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace IBM.Watson.DeveloperCloud.UnitTests
{
    public class TestSpeechToText : UnitTest
    {
        private string _username = null;
        private string _password = null;
        private fsSerializer _serializer = new fsSerializer();
        //private string _token = "<authentication-token>";

        private SpeechToText _speechToText;

        private string _modelNameToGet;
        private string _createdCustomizationID;
        private string _createdCorpusName = "the-jabberwocky-corpus";
        private string _customCorpusFilePath;
        private string _customWordsFilePath;
        private string _acousticResourceUrl = "https://ia802302.us.archive.org/10/items/Greatest_Speeches_of_the_20th_Century/TheFirstAmericaninEarthOrbit.mp3";
        private bool _isAudioLoaded = false;
        private string _createdAcousticModelId;
        private string _acousticResourceName = "unity-acoustic-resource";
        private string _createdAcousticModelName = "unity-example-acoustic-model";
        private byte[] _acousticResourceData;
        private string _acousticResourceMimeType;

        private bool _recognizeTested = false;
        private bool _getModelsTested = false;
        private bool _getModelTested = false;
        private bool _getCustomizationsTested = false;
        private bool _createCustomizationsTested = false;
        private bool _getCustomizationTested = false;
        private bool _trainCustomizationTested = false;
        //private bool _upgradeCustomizationTested = false;
        private bool _resetCustomizationTested = false;
        private bool _getCustomCorporaTested = false;
        private bool _addCustomCorpusTested = false;
        private bool _getCustomCorpusTested = false;
        private bool _getCustomWordsTested = false;
        private bool _addCustomWordsFromPathTested = false;
        private bool _addCustomWordsFromObjectTested = false;
        private bool _getCustomWordTested = false;
        private bool _deleteCustomWordTested = false;
        private bool _deleteCustomCorpusTested = false;
        private bool _getAcousticCustomizationsTested = false;
        private bool _createAcousticCustomizationsTested = false;
        private bool _getAcousticCustomizationTested = false;
        private bool _trainAcousticCustomizationsTested = false;
        private bool _resetAcousticCustomizationsTested = false;
        private bool _getAcousticResourcesTested = false;
        private bool _getAcousticResourceTested = false;
        private bool _addAcousticResourcesTested = false;
        private bool _isCustomizationReady = false;
        private bool _isAcousticCustomizationReady = false;
        private bool _readyToContinue = false;
        private bool _deleteAcousticCustomizationsTested = false;
        private bool _deleteCustomizationsTested = false;
        private bool _deleteAcousticResource = false;
        private float _delayTimeInSeconds = 10f;

        public override IEnumerator RunTest()
        {
            LogSystem.InstallDefaultReactors();

            VcapCredentials vcapCredentials = new VcapCredentials();
            fsData data = null;

            string result = null;
            string credentialsFilepath = "../sdk-credentials/credentials.json";

            //  Load credentials file if it exists. If it doesn't exist, don't run the tests.
            if (File.Exists(credentialsFilepath))
                result = File.ReadAllText(credentialsFilepath);
            else
                yield break;

            //  Add in a parent object because Unity does not like to deserialize root level collection types.
            result = Utility.AddTopLevelObjectToJson(result, "VCAP_SERVICES");

            //  Convert json to fsResult
            fsResult r = fsJsonParser.Parse(result, out data);
            if (!r.Succeeded)
                throw new WatsonException(r.FormattedMessages);

            //  Convert fsResult to VcapCredentials
            object obj = vcapCredentials;
            r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
            if (!r.Succeeded)
                throw new WatsonException(r.FormattedMessages);

            //  Set credentials from imported credntials
            Credential credential = vcapCredentials.GetCredentialByname("speech-to-text-sdk")[0].Credentials;
            _username = credential.Username.ToString();
            _password = credential.Password.ToString();
            _url = credential.Url.ToString();

            //  Create credential and instantiate service
            Credentials credentials = new Credentials(_username, _password, _url);

            //  Or authenticate using token
            //Credentials credentials = new Credentials(_url)
            //{
            //    AuthenticationToken = _token
            //};

            _speechToText = new SpeechToText(credentials);
            _customCorpusFilePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/theJabberwocky-utf8.txt";
            _customWordsFilePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/test-stt-words.json";
            _acousticResourceMimeType = Utility.GetMimeType(Path.GetExtension(_acousticResourceUrl));

            Runnable.Run(DownloadAcousticResource());
            while (!_isAudioLoaded)
                yield return null;

            //  Recognize
            Log.Debug("ExampleSpeechToText.Examples()", "Attempting to recognize");
            List<string> keywords = new List<string>();
            keywords.Add("speech");
            _speechToText.KeywordsThreshold = 0.5f;
            _speechToText.InactivityTimeout = 120;
            _speechToText.StreamMultipart = false;
            _speechToText.Keywords = keywords.ToArray();
            _speechToText.Recognize(HandleOnRecognize, OnFail, _acousticResourceData, _acousticResourceMimeType);
            while (!_recognizeTested)
                yield return null;

            //  Get models
            Log.Debug("ExampleSpeechToText.Examples()", "Attempting to get models");
            _speechToText.GetModels(HandleGetModels, OnFail);
            while (!_getModelsTested)
                yield return null;

            //  Get model
            Log.Debug("ExampleSpeechToText.Examples()", "Attempting to get model {0}", _modelNameToGet);
            _speechToText.GetModel(HandleGetModel, OnFail, _modelNameToGet);
            while (!_getModelTested)
                yield return null;

            //  Get customizations
            Log.Debug("ExampleSpeechToText.Examples()", "Attempting to get customizations");
            _speechToText.GetCustomizations(HandleGetCustomizations, OnFail);
            while (!_getCustomizationsTested)
                yield return null;

            //  Create customization
            Log.Debug("ExampleSpeechToText.Examples()", "Attempting create customization");
            _speechToText.CreateCustomization(HandleCreateCustomization, OnFail, "unity-test-customization", "en-US_BroadbandModel", "Testing customization unity");
            while (!_createCustomizationsTested)
                yield return null;

            //  Get customization
            Log.Debug("ExampleSpeechToText.Examples()", "Attempting to get customization {0}", _createdCustomizationID);
            _speechToText.GetCustomization(HandleGetCustomization, OnFail, _createdCustomizationID);
            while (!_getCustomizationTested)
                yield return null;

            //  Get custom corpora
            Log.Debug("ExampleSpeechToText.Examples()", "Attempting to get custom corpora for {0}", _createdCustomizationID);
            _speechToText.GetCustomCorpora(HandleGetCustomCorpora, OnFail, _createdCustomizationID);
            while (!_getCustomCorporaTested)
                yield return null;

            //  Add custom corpus
            Log.Debug("ExampleSpeechToText.Examples()", "Attempting to add custom corpus {1} in customization {0}", _createdCustomizationID, _createdCorpusName);
            string corpusData = File.ReadAllText(_customCorpusFilePath);
            _speechToText.AddCustomCorpus(HandleAddCustomCorpus, OnFail, _createdCustomizationID, _createdCorpusName, true, corpusData);
            while (!_addCustomCorpusTested)
                yield return null;

            //  Get custom corpus
            Log.Debug("ExampleSpeechToText.Examples()", "Attempting to get custom corpus {1} in customization {0}", _createdCustomizationID, _createdCorpusName);
            _speechToText.GetCustomCorpus(HandleGetCustomCorpus, OnFail, _createdCustomizationID, _createdCorpusName);
            while (!_getCustomCorpusTested)
                yield return null;

            //  Wait for customization
            Runnable.Run(CheckCustomizationStatus(_createdCustomizationID));
            while (!_isCustomizationReady)
                yield return null;

            //  Get custom words
            Log.Debug("ExampleSpeechToText.Examples()", "Attempting to get custom words.");
            _speechToText.GetCustomWords(HandleGetCustomWords, OnFail, _createdCustomizationID);
            while (!_getCustomWordsTested)
                yield return null;

            //  Add custom words from path
            Log.Debug("ExampleSpeechToText.Examples()", "Attempting to add custom words in customization {0} using Words json path {1}", _createdCustomizationID, _customWordsFilePath);
            string customWords = File.ReadAllText(_customWordsFilePath);
            _speechToText.AddCustomWords(HandleAddCustomWordsFromPath, OnFail, _createdCustomizationID, customWords);
            while (!_addCustomWordsFromPathTested)
                yield return null;

            //  Wait for customization
            _isCustomizationReady = false;
            Runnable.Run(CheckCustomizationStatus(_createdCustomizationID));
            while (!_isCustomizationReady)
                yield return null;

            //  Add custom words from object
            Words words = new Words();
            Word w0 = new Word();
            List<Word> wordList = new List<Word>();
            w0.word = "mikey";
            w0.sounds_like = new string[1];
            w0.sounds_like[0] = "my key";
            w0.display_as = "Mikey";
            wordList.Add(w0);
            Word w1 = new Word();
            w1.word = "charlie";
            w1.sounds_like = new string[1];
            w1.sounds_like[0] = "char lee";
            w1.display_as = "Charlie";
            wordList.Add(w1);
            Word w2 = new Word();
            w2.word = "bijou";
            w2.sounds_like = new string[1];
            w2.sounds_like[0] = "be joo";
            w2.display_as = "Bijou";
            wordList.Add(w2);
            words.words = wordList.ToArray();

            Log.Debug("ExampleSpeechToText.Examples()", "Attempting to add custom words in customization {0} using Words object", _createdCustomizationID);
            _speechToText.AddCustomWords(HandleAddCustomWordsFromObject, OnFail, _createdCustomizationID, words);
            while (!_addCustomWordsFromObjectTested)
                yield return null;

            //  Wait for customization
            _isCustomizationReady = false;
            Runnable.Run(CheckCustomizationStatus(_createdCustomizationID));
            while (!_isCustomizationReady)
                yield return null;

            //  Get custom word
            Log.Debug("ExampleSpeechToText.Examples()", "Attempting to get custom word {1} in customization {0}", _createdCustomizationID, words.words[0].word);
            _speechToText.GetCustomWord(HandleGetCustomWord, OnFail, _createdCustomizationID, words.words[0].word);
            while (!_getCustomWordTested)
                yield return null;

            //  Train customization
            Log.Debug("ExampleSpeechToText.Examples()", "Attempting to train customization {0}", _createdCustomizationID);
            _speechToText.TrainCustomization(HandleTrainCustomization, OnFail, _createdCustomizationID);
            while (!_trainCustomizationTested)
                yield return null;

            //  Wait for customization
            _isCustomizationReady = false;
            Runnable.Run(CheckCustomizationStatus(_createdCustomizationID));
            while (!_isCustomizationReady)
                yield return null;

            //  Upgrade customization - not currently implemented in service
            //Log.Debug("ExampleSpeechToText.Examples()", "Attempting to upgrade customization {0}", _createdCustomizationID);
            //_speechToText.UpgradeCustomization(HandleUpgradeCustomization, _createdCustomizationID);
            //while (!_upgradeCustomizationTested)
            //    yield return null;

            //  Delete custom word
            Log.Debug("ExampleSpeechToText.Examples()", "Attempting to delete custom word {1} in customization {0}", _createdCustomizationID, words.words[2].word);
            _speechToText.DeleteCustomWord(HandleDeleteCustomWord, OnFail, _createdCustomizationID, words.words[2].word);
            while (!_deleteCustomWordTested)
                yield return null;

            //  Delay
            Log.Debug("ExampleSpeechToText.Examples()", string.Format("Delaying delete environment for {0} sec", _delayTimeInSeconds));
            Runnable.Run(Delay(_delayTimeInSeconds));
            while (!_readyToContinue)
                yield return null;

            _readyToContinue = false;
            //  Delete custom corpus
            Log.Debug("ExampleSpeechToText.Examples()", "Attempting to delete custom corpus {1} in customization {0}", _createdCustomizationID, _createdCorpusName);
            _speechToText.DeleteCustomCorpus(HandleDeleteCustomCorpus, OnFail, _createdCustomizationID, _createdCorpusName);
            while (!_deleteCustomCorpusTested)
                yield return null;

            //  Delay
            Log.Debug("ExampleSpeechToText.Examples()", string.Format("Delaying delete environment for {0} sec", _delayTimeInSeconds));
            Runnable.Run(Delay(_delayTimeInSeconds));
            while (!_readyToContinue)
                yield return null;

            _readyToContinue = false;
            //  Reset customization
            Log.Debug("ExampleSpeechToText.Examples()", "Attempting to reset customization {0}", _createdCustomizationID);
            _speechToText.ResetCustomization(HandleResetCustomization, OnFail, _createdCustomizationID);
            while (!_resetCustomizationTested)
                yield return null;

            //  Delay
            Log.Debug("ExampleSpeechToText.Examples()", string.Format("Delaying delete environment for {0} sec", _delayTimeInSeconds));
            Runnable.Run(Delay(_delayTimeInSeconds));
            while (!_readyToContinue)
                yield return null;

            _readyToContinue = false;
            //  Delete customization
            Log.Debug("ExampleSpeechToText.Examples()", "Attempting to delete customization {0}", _createdCustomizationID);
            _speechToText.DeleteCustomization(HandleDeleteCustomization, OnFail, _createdCustomizationID);
            while (!_deleteCustomizationsTested)
                yield return null;

            //  List acoustic customizations
            Log.Debug("ExampleSpeechToText.Examples()", "Attempting to get acoustic customizations");
            _speechToText.GetCustomAcousticModels(HandleGetCustomAcousticModels, OnFail);
            while (!_getAcousticCustomizationsTested)
                yield return null;

            //  Create acoustic customization
            Log.Debug("ExampleSpeechToText.Examples()", "Attempting to create acoustic customization");
            _speechToText.CreateAcousticCustomization(HandleCreateAcousticCustomization, OnFail, _createdAcousticModelName);
            while (!_createAcousticCustomizationsTested)
                yield return null;

            //  Get acoustic customization
            Log.Debug("ExampleSpeechToText.Examples()", "Attempting to get acoustic customization {0}", _createdAcousticModelId);
            _speechToText.GetCustomAcousticModel(HandleGetCustomAcousticModel, OnFail, _createdAcousticModelId);
            while (!_getAcousticCustomizationTested)
                yield return null;

            while (!_isAudioLoaded)
                yield return null;

            //  Create acoustic resource
            Log.Debug("ExampleSpeechToText.Examples()", "Attempting to create audio resource {1} on {0}", _createdAcousticModelId, _acousticResourceName);
            string mimeType = Utility.GetMimeType(Path.GetExtension(_acousticResourceUrl));
            _speechToText.AddAcousticResource(HandleAddAcousticResource, OnFail, _createdAcousticModelId, _acousticResourceName, mimeType, mimeType, true, _acousticResourceData);
            while (!_addAcousticResourcesTested)
                yield return null;

            //  Wait for customization
            _isAcousticCustomizationReady = false;
            Runnable.Run(CheckAcousticCustomizationStatus(_createdAcousticModelId));
            while (!_isAcousticCustomizationReady)
                yield return null;

            //  List acoustic resources
            Log.Debug("ExampleSpeechToText.Examples()", "Attempting to get audio resources {0}", _createdAcousticModelId);
            _speechToText.GetCustomAcousticResources(HandleGetCustomAcousticResources, OnFail, _createdAcousticModelId);
            while (!_getAcousticResourcesTested)
                yield return null;

            //  Train acoustic customization
            Log.Debug("ExampleSpeechToText.Examples()", "Attempting to train acoustic customization {0}", _createdAcousticModelId);
            _speechToText.TrainAcousticCustomization(HandleTrainAcousticCustomization, OnFail, _createdAcousticModelId, null, true);
            while (!_trainAcousticCustomizationsTested)
                yield return null;

            //  Get acoustic resource
            Log.Debug("ExampleSpeechToText.Examples()", "Attempting to get audio resource {1} from {0}", _createdAcousticModelId, _acousticResourceName);
            _speechToText.GetCustomAcousticResource(HandleGetCustomAcousticResource, OnFail, _createdAcousticModelId, _acousticResourceName);
            while (!_getAcousticResourceTested)
                yield return null;

            //  Wait for customization
            _isAcousticCustomizationReady = false;
            Runnable.Run(CheckAcousticCustomizationStatus(_createdAcousticModelId));
            while (!_isAcousticCustomizationReady)
                yield return null;

            //  Delete acoustic resource
            DeleteAcousticResource();
            while (!_deleteAcousticResource)
                yield return null;

            //  Delay
            Log.Debug("ExampleSpeechToText.Examples()", string.Format("Delaying delete acoustic resource for {0} sec", _delayTimeInSeconds));
            Runnable.Run(Delay(_delayTimeInSeconds));
            while (!_readyToContinue)
                yield return null;

            //  Reset acoustic customization
            Log.Debug("ExampleSpeechToText.Examples()", "Attempting to reset acoustic customization {0}", _createdAcousticModelId);
            _speechToText.ResetAcousticCustomization(HandleResetAcousticCustomization, OnFail, _createdAcousticModelId);
            while (!_resetAcousticCustomizationsTested)
                yield return null;

            //  Delay
            Log.Debug("ExampleSpeechToText.Examples()", string.Format("Delaying delete acoustic customization for {0} sec", _delayTimeInSeconds));
            Runnable.Run(Delay(_delayTimeInSeconds));
            while (!_readyToContinue)
                yield return null;

            //  Delete acoustic customization
            DeleteAcousticCustomization();
            while (!_deleteAcousticCustomizationsTested)
                yield return null;

            //  Delay
            Log.Debug("ExampleSpeechToText.Examples()", string.Format("Delaying complete for {0} sec", _delayTimeInSeconds));
            Runnable.Run(Delay(_delayTimeInSeconds));
            while (!_readyToContinue)
                yield return null;

            Log.Debug("TestSpeechToText.RunTest()", "Speech to Text examples complete.");

            yield break;
        }

        private void DeleteAcousticResource()
        {
            Log.Debug("ExampleSpeechToText.DeleteAcousticResource()", "Attempting to delete audio resource {1} from {0}", _createdAcousticModelId, _acousticResourceName);
            _speechToText.DeleteAcousticResource(HandleDeleteAcousticResource, OnFail, _createdAcousticModelId, _acousticResourceName);
        }

        private void DeleteAcousticCustomization()
        {
            Log.Debug("ExampleSpeechToText.DeleteAcousticCustomization()", "Attempting to delete acoustic customization {0}", _createdAcousticModelId);
            _speechToText.DeleteAcousticCustomization(HandleDeleteAcousticCustomization, OnFail, _createdAcousticModelId);
        }

        private void HandleGetModels(ModelSet result, Dictionary<string, object> customData)
        {

            Log.Debug("ExampleSpeechToText.HandleGetModels()", "{0}", customData["json"].ToString());
            _modelNameToGet = (result.models[UnityEngine.Random.Range(0, result.models.Length - 1)] as Model).name;
            Test(result != null);
            _getModelsTested = true;
        }

        private void HandleGetModel(Model model, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleSpeechToText.HandleGetModel()", "{0}", customData["json"].ToString());
            Test(model != null);
            _getModelTested = true;
        }

        private void HandleOnRecognize(SpeechRecognitionEvent result, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleSpeechToText.HandleOnRecognize()", "{0}", customData["json"].ToString());
            Test(result != null);
            _recognizeTested = true;
        }

        private void HandleGetCustomizations(Customizations customizations, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleSpeechToText.HandleGetCustomizations()", "{0}", customData["json"].ToString());
            Test(customizations != null);
            _getCustomizationsTested = true;
        }

        private void HandleCreateCustomization(CustomizationID customizationID, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleSpeechToText.HandleCreateCustomization()", "{0}", customData["json"].ToString());
            _createdCustomizationID = customizationID.customization_id;
            Test(customizationID != null);
            _createCustomizationsTested = true;
        }

        private void HandleGetCustomization(Customization customization, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleSpeechToText.HandleGetCustomization()", "{0}", customData["json"].ToString());
            Test(customization != null);
            _getCustomizationTested = true;
        }

        private void HandleDeleteCustomization(bool success, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleSpeechToText.HandleDeleteCustomization()", "{0}", customData["json"].ToString());
            _createdCustomizationID = default(string);
            Test(success);
            _deleteCustomizationsTested = true;
        }

        private void HandleTrainCustomization(bool success, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleSpeechToText.HandleTrainCustomization()", "{0}", customData["json"].ToString());
            Test(success);
            _trainCustomizationTested = true;
        }

        //private void HandleUpgradeCustomization(bool success, Dictionary<string, object> customData)
        //{
        //     Test(success);
        //    _upgradeCustomizationTested = true;
        //}

        private void HandleResetCustomization(bool success, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleSpeechToText.HandleResetCustomization()", "{0}", customData["json"].ToString());
            Test(success);
            _resetCustomizationTested = true;
        }

        private void HandleGetCustomCorpora(Corpora corpora, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleSpeechToText.HandleGetCustomCorpora()", "{0}", customData["json"].ToString());
            Test(corpora != null);
            _getCustomCorporaTested = true;
        }

        private void HandleDeleteCustomCorpus(bool success, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleSpeechToText.HandleDeleteCustomCorpus()", "{0}", customData["json"].ToString());
            Test(success);
            _deleteCustomCorpusTested = true;
        }

        private void HandleAddCustomCorpus(bool success, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleSpeechToText.HandleAddCustomCorpus()", "{0}", customData["json"].ToString());
            Test(success);
            _addCustomCorpusTested = true;
        }

        private void HandleGetCustomCorpus(Corpus corpus, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleSpeechToText.HandleGetCustomCorpus()", "{0}", customData["json"].ToString());
            Test(corpus != null);
            _getCustomCorpusTested = true;
        }

        private void HandleGetCustomWords(WordsList wordList, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleSpeechToText.HandleGetCustomWords()", "{0}", customData["json"].ToString());
            Test(wordList != null);
            _getCustomWordsTested = true;
        }

        private void HandleAddCustomWordsFromPath(bool success, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleSpeechToText.HandleAddCustomWordsFromPath()", "{0}", customData["json"].ToString());
            Test(success);
            _addCustomWordsFromPathTested = true;
        }

        private void HandleAddCustomWordsFromObject(bool success, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleSpeechToText.HandleAddCustomWordsFromObject()", "{0}", customData["json"].ToString());
            Test(success);
            _addCustomWordsFromObjectTested = true;
        }

        private void HandleDeleteCustomWord(bool success, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleSpeechToText.HandleDeleteCustomWord()", "{0}", customData["json"].ToString());
            Test(success);
            _deleteCustomWordTested = true;
        }

        private void HandleGetCustomWord(WordData word, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleSpeechToText.HandleGetCustomWord()", "{0}", customData["json"].ToString());
            Test(word != null);
            _getCustomWordTested = true;
        }

        private void HandleGetCustomAcousticModels(AcousticCustomizations acousticCustomizations, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleSpeechToText.HandleGetCustomAcousticModels()", "{0}", customData["json"].ToString());
            Test(acousticCustomizations != null);
            _getAcousticCustomizationsTested = true;
        }

        private void HandleCreateAcousticCustomization(CustomizationID customizationID, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleSpeechToText.HandleCreateAcousticCustomization()", "{0}", customData["json"].ToString());
            _createdAcousticModelId = customizationID.customization_id;
            Test(customizationID != null);
            _createAcousticCustomizationsTested = true;
        }

        private void HandleGetCustomAcousticModel(AcousticCustomization acousticCustomization, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleSpeechToText.HandleGetCustomAcousticModel()", "{0}", customData["json"].ToString());
            Test(acousticCustomization != null);
            _getAcousticCustomizationTested = true;
        }

        private void HandleTrainAcousticCustomization(bool success, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleSpeechToText.HandleTrainAcousticCustomization()", "{0}", customData["json"].ToString());
            Test(success);
            _trainAcousticCustomizationsTested = true;
        }

        private void HandleGetCustomAcousticResources(AudioResources audioResources, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleSpeechToText.HandleGetCustomAcousticResources()", "{0}", customData["json"].ToString());
            Test(audioResources != null);
            _getAcousticResourcesTested = true;
        }

        private void HandleAddAcousticResource(bool success, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleSpeechToText.HandleAddAcousticResource()", "{0}", customData["json"].ToString());
            Test(success);
            _addAcousticResourcesTested = true;
        }

        private void HandleGetCustomAcousticResource(AudioListing audioListing, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleSpeechToText.HandleGetCustomAcousticResource()", "{0}", customData["json"].ToString());
            Test(audioListing != null);
            _getAcousticResourceTested = true;
        }

        private void HandleResetAcousticCustomization(bool success, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleSpeechToText.HandleResetAcousticCustomization()", "{0}", customData["json"].ToString());
            Test(success);
            _resetAcousticCustomizationsTested = true;
        }

        private void HandleDeleteAcousticResource(bool success, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleSpeechToText.HandleDeleteAcousticResource()", "{0}", customData["json"].ToString());
            Test(success);
            _deleteAcousticResource = true;
        }

        private void HandleDeleteAcousticCustomization(bool success, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleSpeechToText.HandleDeleteAcousticCustomization()", "{0}", customData["json"].ToString());
            Test(success);
            _deleteAcousticCustomizationsTested = true;
        }

        private IEnumerator CheckCustomizationStatus(string customizationID, float delay = 0.1f)
        {
            Log.Debug("ExampleSpeechToText.CheckCustomizationStatus()", "Checking customization status in {0} seconds...", delay.ToString());
            yield return new WaitForSeconds(delay);

            //  passing customizationID in custom data
            Dictionary<string, object> customData = new Dictionary<string, object>();
            customData["customizationID"] = customizationID;
            _speechToText.GetCustomization(OnCheckCustomizationStatus, OnFail, customizationID, customData);
        }

        private void OnCheckCustomizationStatus(Customization customization, Dictionary<string, object> customData)
        {
            if (customization != null)
            {
                Log.Debug("ExampleSpeechToText.OnCheckCustomizationStatus()", "Customization status: {0}", customization.status);
                if (customization.status != "ready" && customization.status != "available")
                    Runnable.Run(CheckCustomizationStatus(customData["customizationID"].ToString(), 5f));
                else
                    _isCustomizationReady = true;
            }
            else
            {
                Log.Debug("ExampleSpeechToText.OnCheckCustomizationStatus()", "Check customization status failed!");
            }
        }

        private IEnumerator CheckAcousticCustomizationStatus(string customizationID, float delay = 0.1f)
        {
            Log.Debug("ExampleSpeechToText.CheckAcousticCustomizationStatus()", "Checking acoustic customization status in {0} seconds...", delay.ToString());
            yield return new WaitForSeconds(delay);

            //	passing customizationID in custom data
            Dictionary<string, object> customData = new Dictionary<string, object>();
            customData["customizationID"] = customizationID;
            _speechToText.GetCustomAcousticModel(OnCheckAcousticCustomizationStatus, OnFail, customizationID, customData);
        }

        private void OnCheckAcousticCustomizationStatus(AcousticCustomization acousticCustomization, Dictionary<string, object> customData)
        {
            if (acousticCustomization != null)
            {
                Log.Debug("ExampleSpeechToText.CheckAcousticCustomizationStatus()", "Acoustic customization status: {0}", acousticCustomization.status);
                if (acousticCustomization.status != "ready" && acousticCustomization.status != "available")
                    Runnable.Run(CheckAcousticCustomizationStatus(customData["customizationID"].ToString(), 5f));
                else
                    _isAcousticCustomizationReady = true;
            }
            else
            {
                Log.Debug("ExampleSpeechToText.CheckAcousticCustomizationStatus()", "Check acoustic customization status failed!");
            }
        }

        private IEnumerator Delay(float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            _readyToContinue = true;
        }

        private IEnumerator DownloadAcousticResource()
        {
            Log.Debug("ExampleSpeechToText.DownloadAcousticResource()", "downloading acoustic resource from {0}", _acousticResourceUrl);
            WWW www = new WWW(_acousticResourceUrl);
            yield return www;

            Log.Debug("ExampleSpeechToText.DownloadAcousticResource()", "acoustic resource downloaded");
            _acousticResourceData = www.bytes;
            _isAudioLoaded = true;
            www.Dispose();
        }

        private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
        {
            Log.Error("ExampleSpeechToText.OnFail()", "Error received: {0}", error.ToString());
        }
    }
}
