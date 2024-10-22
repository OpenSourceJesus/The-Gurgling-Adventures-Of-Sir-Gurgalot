/**
* Copyright 2018 IBM Corp. All Rights Reserved.
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
using System.Collections.Generic;

namespace IBM.Watson.DeveloperCloud.Services.Discovery.v1
{
    /// <summary>
    /// The query expansion definitions for the specified collection.
    /// </summary>
    [fsObject]
    public class Expansions
    {
        /// <summary>
        /// An array of query expansion definitions.
        ///
        ///  Each object in the **expansions** array represents a term or set of terms that will be expanded into other
        /// terms. Each expansion object can be configured as bidirectional or unidirectional. Bidirectional means that
        /// all terms are expanded to all other terms in the object. Unidirectional means that a set list of terms can
        /// be expanded into a second list of terms.
        ///
        ///  To create a bi-directional expansion specify an **expanded_terms** array. When found in a query, all items
        /// in the **expanded_terms** array are then expanded to the other items in the same array.
        ///
        ///  To create a uni-directional expansion, specify both an array of **input_terms** and an array of
        /// **expanded_terms**. When items in the **input_terms** array are present in a query, they are expanded using
        /// the items listed in the **expanded_terms** array.
        /// </summary>
        [fsProperty("expansions")]
        public List<Expansion> ExpansionsProperty { get; set; }
    }

}
