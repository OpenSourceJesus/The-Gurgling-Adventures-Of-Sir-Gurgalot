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

using System.Collections;
using System.Collections.Generic;
using IBM.Watson.DeveloperCloud.Services.Conversation.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Logging;
using FullSerializer;
using System.IO;
using IBM.Watson.DeveloperCloud.Connection;

namespace IBM.Watson.DeveloperCloud.UnitTests
{
    public class TestConversation : UnitTest
    {
        private string _username = null;
        private string _password = null;
        private string _workspaceId = "506e4a2e-3d5d-4dca-b374-38edbb4139ab";
        //private string _token = "<authentication-token>";

        private Conversation _conversation;
        private string _conversationVersionDate = "2017-05-26";

        private string[] _questionArray = { "can you turn up the AC", "can you turn on the wipers", "can you turn off the wipers", "can you turn down the ac", "can you unlock the door" };
        private fsSerializer _serializer = new fsSerializer();
        private Dictionary<string, object> _context = null;
        private int _questionCount = -1;
        private bool _waitingForResponse = true;
        private bool _deleteUserDataTested = false;

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
            Credential credential = vcapCredentials.GetCredentialByname("conversation-sdk")[0].Credentials;
            _username = credential.Username.ToString();
            _password = credential.Password.ToString();
            _url = credential.Url.ToString();
            //_workspaceId = credential.WorkspaceId.ToString();

            //  Create credential and instantiate service
            Credentials credentials = new Credentials(_username, _password, _url);

            _conversation = new Conversation(credentials);
            _conversation.VersionDate = _conversationVersionDate;

            //  Test initate with empty string
            if (!_conversation.Message(OnSuccess, OnFail, _workspaceId, ""))
                Log.Debug("TestConversation.RunTest()", "Failed to message!");

            //  Test initiate with empty string message object
            MessageRequest messageRequest = new MessageRequest()
            {
                input = new Dictionary<string, object>()
            {
                { "text", "" }
            },
                context = _context
            };

            if (!_conversation.Message(OnSuccess, OnFail, _workspaceId, messageRequest))
                Log.Debug("TestConversation.RunTest()", "Failed to message!");

            if (!_conversation.Message(OnSuccess, OnFail, _workspaceId, "hello"))
                Log.Debug("TestConversation.RunTest()", "Failed to message!");

            while (_waitingForResponse)
                yield return null;

            _waitingForResponse = true;
            _questionCount++;

            AskQuestion();
            while (_waitingForResponse)
                yield return null;

            _questionCount++;

            _waitingForResponse = true;

            AskQuestion();
            while (_waitingForResponse)
                yield return null;
            _questionCount++;

            _waitingForResponse = true;

            AskQuestion();
            while (_waitingForResponse)
                yield return null;
            _questionCount++;

            _waitingForResponse = true;

            AskQuestion();
            while (_waitingForResponse)
                yield return null;

            //  Delete User Data
            _conversation.DeleteUserData(OnDeleteUserData, OnFail, "test-unity-user-id");
            while (!_deleteUserDataTested)
                yield return null;

            Log.Debug("TestConversation.RunTest()", "Conversation examples complete.");

            yield break;
        }

        private void AskQuestion()
        {
            MessageRequest messageRequest = new MessageRequest()
            {
                input = new Dictionary<string, object>()
            {
                { "text", _questionArray[_questionCount] }
            },
                context = _context
            };

            if (!_conversation.Message(OnSuccess, OnFail, _workspaceId, messageRequest))
                Log.Debug("TestConversation.AskQuestion()", "Failed to message!");
        }

        private void OnSuccess(object resp, Dictionary<string, object> customData)
        {
            Log.Debug("TestConversation.OnMessage()", "Conversation: Message Response: {0}", customData["json"].ToString());

            //  Convert resp to fsdata
            fsData fsdata = null;
            fsResult r = _serializer.TrySerialize(resp.GetType(), resp, out fsdata);
            if (!r.Succeeded)
                throw new WatsonException(r.FormattedMessages);

            //  Convert fsdata to MessageResponse
            MessageResponse messageResponse = new MessageResponse();
            object obj = messageResponse;
            r = _serializer.TryDeserialize(fsdata, obj.GetType(), ref obj);
            if (!r.Succeeded)
                throw new WatsonException(r.FormattedMessages);

            object _tempContext = null;
            //  Set context for next round of messaging
            (resp as Dictionary<string, object>).TryGetValue("context", out _tempContext);

            if (_tempContext != null)
                _context = _tempContext as Dictionary<string, object>;
            else
                Log.Debug("TestConversation.OnMessage()", "Failed to get context");

            Test(messageResponse != null);
            _waitingForResponse = false;
        }

        private void OnDeleteUserData(object response, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleAssistant.OnDeleteUserData()", "Response: {0}", customData["json"].ToString());
            _deleteUserDataTested = true;
        }

        private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
        {
            Log.Error("TestConversation.OnFail()", "Error received: {0}", error.ToString());
        }
    }
}
