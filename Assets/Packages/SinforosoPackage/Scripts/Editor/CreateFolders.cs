using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateFolders
{
    #region membros

    private static readonly string[] _folders = { "Animations", "Animators", "AudioClips", "AudioMixers", "AvatarMasks", "Fonts", "LightingSettings", "Materials", "Models", "PhysicsMaterials", "Prefabs", "RenderTextures", "Scenes", "Scriptables", "Scripts", "Shaders", "Sprites", "VolumeProfiles" };

    private static readonly string[] _scriptSubfolders = { "_Drawers", "_Enums", "_Interfaces", "_Managers", "_Scriptables", "_Serializables", "Editor" };
    private static readonly string[] _spriteSubfolders = { "_NormalMaps", "_Textures" };

    #endregion

    [MenuItem("Folder/_CreateME")]
    private static void CreateMe()
    {
        string folderFiles = Path.Combine(Application.dataPath, "_Files");
        string folderMe = Path.Combine(Application.dataPath, "_ME");

        if (!Directory.Exists(folderFiles))
        {
            Directory.CreateDirectory(folderFiles);
        }

        if (!Directory.Exists(folderMe))
        {
            Directory.CreateDirectory(folderMe);

            for (int i = 0; i < _folders.Length; i++)
            {
                string _newFolder = Path.Combine(folderMe, _folders[i]);
                Directory.CreateDirectory(_newFolder);

                if (_folders[i] == "Scripts")
                {
                    foreach (string _subfolder in _scriptSubfolders)
                    {
                        Directory.CreateDirectory(Path.Combine(_newFolder, _subfolder));
                    }
                }
                else if (_folders[i] == "Sprites")
                {
                    foreach (string _subfolder in _spriteSubfolders)
                    {
                        Directory.CreateDirectory(Path.Combine(_newFolder, _subfolder));
                    }
                }
            }

            Debug.Log("Pastas Criadas.");
            return;
        }

        Debug.Log($"A pasta ja existe em: {folderMe}");
    }

    [MenuItem("Folder/_CreateSinforoso")]
    private static void FolderShared_Create()
    {
        string folderSinforoso = Path.Combine(Application.dataPath, "_Sinforoso");

        if (!Directory.Exists(folderSinforoso))
        {
            Directory.CreateDirectory(folderSinforoso);

            for (int i = 0; i < _folders.Length; i++)
            {
                string newFolder = Path.Combine(folderSinforoso, _folders[i]);
                Directory.CreateDirectory(newFolder);

                if (_folders[i] == "Scripts")
                {
                    foreach (string subfolder in _scriptSubfolders)
                    {
                        Directory.CreateDirectory(Path.Combine(newFolder, subfolder));
                    }
                }
                else if (_folders[i] == "Sprites")
                {
                    foreach (string subfolder in _spriteSubfolders)
                    {
                        Directory.CreateDirectory(Path.Combine(newFolder, subfolder));
                    }
                }
            }

            Debug.Log("Pastas Criadas.");
            return;
        }

        Debug.Log($"A pasta ja existe em: {folderSinforoso}");
    }
}
