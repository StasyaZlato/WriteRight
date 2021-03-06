﻿using System;

namespace ProjectStructure
{
    [Serializable]
    public class FileOfProject
    {
        public string Name { get; set; }
        public string PathInProject { get; set; }
        public string NameOfFile { get; set; }
        public string PathToFile { get; set; }

        public FileOfProject() { }

        public FileOfProject(string name, string path, int num)
        {
            Name = name;
            PathToFile = path;
            NameOfFile = $"{num}.xml";
            if (Section.CheckInvalidFileName(name))
            {
                throw new IncorrectNameOfFileException("Название содержит недопустимые символы");
            }
        }

        public FileOfProject(string name, int num)
        {
            Name = name;
            NameOfFile = $"{num}.xml";
            if (Section.CheckInvalidFileName(name))
            {
                throw new IncorrectNameOfFileException("Название содержит недопустимые символы");
            }
        }
    }
}