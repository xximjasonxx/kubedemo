pipeline {
  agent {
    docker {
      image 'xximjasonxx/kubedemo'
    }

  }
  stages {
    stage('build price generator') {
      steps {
        sh 'cd PriceGenerator'
      }
    }
  }
}