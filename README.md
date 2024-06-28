### 

# 3D Multi FPS


---

## Description

---


- 🔊프로젝트 소개
  
  3D Multi FPS는 멀티플레이 환경을 제공하는 3D FPS게임입니다. 회원가입 및 로그인, 방 생성, 방 입장, 게임시작 및 객체의 트랜스폼, 애니메이션, 생성 및 제거, 데이터 등 기초적인 동기화 작업에 중점을 두었습니다.

- 개발 기간 : 2024.03.27 - 2024.03.31

- 🛠️사용 기술 및 개발 환경

   -언어 : C#
  
   -엔진 : Unity Engine
  
   -데이터베이스 : 로컬
  
   -개발 환경 : Window 10, Unity 2021.3.10f1



- 💻구동 화면

![스크린샷(2)](https://github.com/oyb1412/3DMultiFps/assets/154235801/757a3956-c045-4e60-b9ab-e3e3966dc0fe)

## 목차

---

- 기획 의도
- 핵심 로직


### 기획 의도

---

- 멀티플레이 환경 구축

- 회원 등록, 로그인, 방 생성, 방 입장, 게임 시작 등의 흐름 구축
  

### 핵심 로직

---
![Line_1_(1)](https://github.com/oyb1412/TinyDefense/assets/154235801/f664c47e-d52b-4980-95ec-9859dea11aab)
### ・멀티플레이를 위한 유닛간 동기화

유닛간의 이동, 회전, 물리처리 및 애니메이션과 같은 표면적인 정보의 동기화 및 점수, 체력등의 내적인 데이터를 동기화해 자연스러운 멀티플레이 경험을 제공

![그림1](https://github.com/oyb1412/3DMultiFps/assets/154235801/e51b40c9-327f-433e-89ce-8210670fcd67)
![Line_1_(1)](https://github.com/oyb1412/TinyDefense/assets/154235801/f664c47e-d52b-4980-95ec-9859dea11aab)

### ・멀티플레이를 위한 로그인, 방 생성, 입장, 게임시작 기능


같이 플레이를 원하는 플레이어와의 플레이를 위해 방 기능을 구현

![그림2](https://github.com/oyb1412/3DMultiFps/assets/154235801/6de25b39-add8-4834-b535-f27c9a5820bc)
![Line_1_(1)](https://github.com/oyb1412/TinyDefense/assets/154235801/f664c47e-d52b-4980-95ec-9859dea11aab)
### ・유니티 에디터를 이용한 AI의 시야를 외적으료 표현

‘적의 시야각 90도’ 라는 단순한 정보만이 아닌, 실제 눈으로 확인 가능한 시야각을 에디터를 사용해 제공

![그림3](https://github.com/oyb1412/3DMultiFps/assets/154235801/db3d5ddd-c0f6-4020-98a0-5ee106d10f05)
![Line_1_(1)](https://github.com/oyb1412/TinyDefense/assets/154235801/f664c47e-d52b-4980-95ec-9859dea11aab)

### ・옵저버 패턴을 이용한 UI 시스템

데이터 변경이 없을 때도 주기적으로 UI를 업데이트하는 문제를 해결하여 퍼포먼스를 최적화

![그림4](https://github.com/oyb1412/3DMultiFps/assets/154235801/47874b0f-079d-46bb-ac0e-1fc7f1adcca1)
![Line_1_(1)](https://github.com/oyb1412/TinyDefense/assets/154235801/f664c47e-d52b-4980-95ec-9859dea11aab)

### ・풀링 오브젝트 시스템

런타임 시 객체 생성과 제거를 방지하고, 성능을 높이기 위해 풀링 시스템 사용.

![화면 캡처 2024-06-28 161519](https://github.com/oyb1412/3DMultiFps/assets/154235801/72184c18-7992-4e18-9466-31d9fc06879e)

