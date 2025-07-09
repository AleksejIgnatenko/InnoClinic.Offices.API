pipeline {
    agent any

    environment {
        SOLUTION_FILE = 'InnoClinic.Offices.API.sln'
    }

    stages {
        stage('Clone Repository') {
            steps {
                git branch: 'master', url: 'https://github.com/AleksejIgnatenko/InnoClinic.Offices.API '
            }
        }

        stage('Restore') {
            steps {
                sh "dotnet restore ${env.SOLUTION_FILE}"
            }
        }

        stage('Build') {
            steps {
                sh "dotnet build ${env.SOLUTION_FILE} --configuration Release"
            }
        }

        stage('Test') {
            steps {
                sh "dotnet test ${env.SOLUTION_FILE} --configuration Release"
            }
        }
    }
}
