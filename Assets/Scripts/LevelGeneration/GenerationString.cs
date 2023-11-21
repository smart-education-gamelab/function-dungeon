using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine;

//This class is resposible for serializing and deserializing generation strings based on the GenerationVariables class.
public static class GenerationString {
    private static int VERSION = 2;

    private class VersionedGenerationVariables {
        [JsonProperty("V")]
        public int version = VERSION;
        [JsonProperty("v")]
        public GenerationVariables variables;

        public VersionedGenerationVariables(GenerationVariables variables) {
            this.variables = variables;
        }
    }

    public static string Serialize(GenerationVariables variables) {
        return Utils.Base64Encode(JsonConvert.SerializeObject(new VersionedGenerationVariables(variables), Formatting.None));
    }

    public static GenerationVariables Deserialize(string data) {
        try {
            VersionedGenerationVariables versioned = JObject.Parse(Utils.Base64Decode(data)).ToObject<VersionedGenerationVariables>();
            if (versioned == null) {
                Debug.Log($"Failed to deserialize generation variables");
                return null;
            }
            if (versioned.version != VERSION) {
                Debug.Log($"Generation variables version mismatch! Saved version is {versioned.version} and current version is {VERSION}");
                return null;
            }
            return versioned.variables;
        } catch (Exception) {
            return null;
        }
    }

}
