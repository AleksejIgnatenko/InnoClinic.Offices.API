pipeline {
    agent any 

    environment {
        SOLUTION_FILE = 'InnoClinic.Offices.API.sln'
        DOTNET_IMAGE  = 'mcr.microsoft.com/dotnet/sdk:8.0'
    }

    stages {
        stage('Clone Repository') {
            steps {
                git branch: 'master', url: 'https://github.com/AleksejIgnatenko/InnoClinic.Offices.API '
            }
        }

        stage('Restore') {
            steps {
                sh """
                  docker run --rm -v \$(pwd):/src -w /src ${env.DOTNET_IMAGE} \\
                    dotnet restore ${env.SOLUTION_FILE}
                """
            }
        }

        stage('Build') {
            steps {
                sh """
                  docker run --rm -v \$(pwd):/src -w /src ${env.DOTNET_IMAGE} \\
                    dotnet build ${env.SOLUTION_FILE} --configuration Release
                """
            }
        }

        stage('Test') {
            steps {
                sh """
                  docker run --rm -v \$(pwd):/src -w /src ${env.DOTNET_IMAGE} \\
                    dotnet test ${env.SOLUTION_FILE} --configuration Release
                """
            }
        }
    }
}
