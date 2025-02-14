plugins {
    java
    id("org.springframework.boot") version "3.4.2"
    id("io.spring.dependency-management") version "1.1.7"
}

group = "payment.service"
version = "0.0.1-SNAPSHOT"

subprojects{
    apply(plugin = "java")
    apply(plugin = "io.spring.dependency-management")

    repositories {
        mavenCentral()
    }

    dependencyManagement {
        imports {
            mavenBom("org.springframework.modulith:spring-modulith-bom:1.3.1")
            mavenBom("org.springframework.boot:spring-boot-dependencies:3.4.2")
        }
    }

    java {
        toolchain {
            languageVersion = JavaLanguageVersion.of(23)
        }
    }
}

java {
    toolchain {
        languageVersion = JavaLanguageVersion.of(23)
    }
}

configurations {
    compileOnly {
        extendsFrom(configurations.annotationProcessor.get())
    }
}

repositories {
    mavenCentral()
}

extra["springModulithVersion"] = "1.3.1"

dependencies {
    implementation("org.springframework.modulith:spring-modulith-starter-core")
    developmentOnly("org.springframework.boot:spring-boot-devtools")
    annotationProcessor("org.springframework.boot:spring-boot-configuration-processor")
}

dependencyManagement {
    imports {
        mavenBom("org.springframework.modulith:spring-modulith-bom:${property("springModulithVersion")}")
    }
}

tasks.wrapper{
    gradleVersion = "8.12.1"
    distributionType = Wrapper.DistributionType.BIN
}
