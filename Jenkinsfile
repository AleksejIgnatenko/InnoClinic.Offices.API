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