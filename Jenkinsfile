pipeline {
    agent any

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