pipeline {
    agent any

    environment {
        DOTNET_VERSION = '8.0'
    }

    stages {
        stage('Clone Repository') {
            steps {
                git branch: 'main', url: 'https://github.com/AleksejIgnatenko/InnoClinic.Offices.API '
            }
        }

        stage('Build with .NET') {
            steps {
                script {
                    docker.image("mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION}")
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