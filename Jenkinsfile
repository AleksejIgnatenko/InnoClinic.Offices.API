pipeline {
    agent any

    stages {
        stage('Clone Repository') {
            steps {
                git branch: 'master', url: 'https://github.com/AleksejIgnatenko/InnoClinic.Offices.API '
            }
        }

        stage('Build with .NET') {
            steps {
                script {
                    docker.image("mcr.microsoft.com/dotnet/sdk:8.0")
                        .inside("-v /tmp:/tmp") {
                            sh 'dotnet restore InnoClinic.Offices.API.sln'
                            sh 'dotnet build InnoClinic.Offices.API.sln --configuration Release'
                            sh 'dotnet test InnoClinic.Offices.API.sln --configuration Release'
                        }
                }
            }
        }
    }
}