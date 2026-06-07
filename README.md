# Lotto (WinForms) - Archive

![Status](https://img.shields.io/badge/Status-Archived-red)
![C#](https://img.shields.io/badge/Language-C%23-blue)
![.NET](https://img.shields.io/badge/Framework-.NET%204.7.2-green)
![SQLite](https://img.shields.io/badge/Database-SQLite-lightgrey)

이 프로젝트는 2023년에 개발된 **C# WinForms 기반 로또 구매 및 추첨 시스템** 으로, 대학 축제 부스 운영 목적으로 개발하였습니다.
현재는 더 이상 업데이트나 유지보수를 진행하지 않으며, 학습 및 과거 기록을 보존하는 **아카이브(Archive)** 용도로 업로드 하였습니다.

## 프로젝트 개요
*   **개발 기간**: 2023.09.10 ~ 2023.09.11
*   **주요 용도**: 학생회 행사 및 축제 당시 로또 번호 응모 및 추첨 관리
*   **프로젝트 상태**: **Deprecated / Archived**

## 주요 기술 스택
*   **Language**: C# 
*   **Framework**: .NET Framework 4.7.2
*   **UI Library**: Bunifu UI WinForms 5.0.3
*   **Database**: SQLite
*   **Web Integration**: WebView2 (로딩 화면), Discord Webhook (알림)

## 주요 기능
1.  **번호 응모 (lotto_input_form)**
    *   구매자 학번/연락처 기반 번호 입력 및 DB 저장
    *   수동 입력 및 랜덤 번호 자동 입력 지원
    *   WebView2를 활용한 인터랙티브 로딩 UI
      
2.  **당첨 추첨 (admin_raffle)**
    *   관리자용 실시간 번호 추첨 인터페이스
    *   저장된 응모 데이터 기반 당첨자(1~3등) 자동 판별
    *   대량의 테스트 데이터 생성 기능 포함
      
3.  **알림 시스템 (SharedUtils)**
    *   입력 정보를 Discord 채널로 즉시 전송 (Webhook)

## 프로젝트 구조
*   `admin_raffle.cs`: 당첨 추첨 및 관리자 기능
*   `lotto_input_form.cs`: 로또 구매 폼
*   `SharedUtils.cs`: DB 초기화, Webhook 전송 등 공용 유틸리티
*   `choice.cs`: 메인 메뉴 선택 화면

## 주의사항
*   **환경 설정**: 실행을 위해 `WebView2`, `Bunifu UI`, `SQLite` 라이브러리를 설치하여야 하며  `Bunifu UI` 는 라이선스가 필요합니다.
*   **데이터베이스**: 실행 시 `lotto.db` 파일이 자동 생성됩니다.
*   **보안**: `SharedUtils.cs` 내의 `WebhookUrl` 은 실제 사용하는 Discord Webhook URL을 입력하시면 됩니다.(없어도 무관)


