pipeline {
    agent any

    environment {
        DOTNET_VERSION = '8.0'
    }

    stages {
        stage('Clone Repository') {
            steps {
                git branch: 'master', url: 'https://github.com/AleksejIgnatenko/InnoClinic.Offices.API'
            }
        }

        stage('Restore') {
            steps {
                sh 'dotnet restore InnoClinic.Offices.API.sln'
            }
        }

        stage('Build') {
            steps {
                sh 'dotnet build InnoClinic.Offices.API.sln --configuration Release'
            }
        }

        stage('Test') {
            steps {
                sh 'dotnet test InnoClinic.Offices.API.sln --configuration Release'
            }
        }
    }
}