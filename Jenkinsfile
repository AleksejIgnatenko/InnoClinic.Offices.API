pipeline {
    agent any

    stages {
        stage('Clone Repository') {
            steps {
                git branch: 'main', url: 'https://github.com/ ваш-аккаунт/ваш-репозиторий.git'
            }
        }

        stage('Restore') {
            steps {
                sh 'dotnet restore YourSolution.sln'
            }
        }

        stage('Build') {
            steps {
                sh 'dotnet build YourSolution.sln --configuration Release'
            }
        }

        stage('Test') {
            steps {
                sh 'dotnet test YourSolution.sln --configuration Release'
            }
        }
    }
}