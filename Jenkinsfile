pipeline {
    agent any  // Use any available agent

    environment {
        SOLUTION_FILE = 'InnoClinic.Offices.API.sln'
        DOTNET_IMAGE    = 'mcr.microsoft.com/dotnet/sdk:8.0'
    }

    stages {
        stage('Clone Repository') {
            steps {
                git branch: 'master', url: 'https://github.com/AleksejIgnatenko/InnoClinic.Offices.API '
            }
        }

        stage('Build in Docker') {
            steps {
                script {
                    docker.image(env.DOTNET_IMAGE).inside {
                        sh "dotnet restore ${env.SOLUTION_FILE}"
                        sh "dotnet build ${env.SOLUTION_FILE} --configuration Release"
                        sh "dotnet test ${env.SOLUTION_FILE} --configuration Release"
                    }
                }
            }
        }
    }
}