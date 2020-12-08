# TheCastle-OpenGL

###### Kod je u direktorijumu *AssimpSample*

<details>
  <summary> Wireframe </summary> <br>
  
![image](https://user-images.githubusercontent.com/45834270/101497378-2f226800-396b-11eb-98c6-f92398f7e597.png)
</details>

<details>
  <summary> Korisne reference </summary> <br>
  
  - pogodni sajtovi za izbor 3D modela 
    - [sajt](https://clara.io/library)
    - [sajt](https://free3d.com/) 
</details>

<br><br>
<details>
  <summary> Tehnicki problemi </summary> <br>

<details>
  <summary> Problem sa putanjom modela </summary> <br>
  
![image](https://user-images.githubusercontent.com/45834270/101499598-bd97e900-396d-11eb-8d42-303c554fe557.png)

Za ovakav 3D Models, ako sam u **rezimu debug**, potrebno je da u direktorijum bin/Debug ubacim takodje 3D Models pre pokretanja:
  - znaci build-ujem projekat
  - ubacim 3D Models u bin/Debug 
  
![image](https://user-images.githubusercontent.com/45834270/101499852-0a7bbf80-396e-11eb-9247-78b25bd273ce.png)

![image](https://user-images.githubusercontent.com/45834270/101499886-136c9100-396e-11eb-9b74-7a68e418213a.png)



</details>


<details>
  <summary> Problem sa Assimp32.dll </summary> <br>
  
  - ovo je resilo [problem](https://stackoverflow.com/questions/44764577/cannot-use-assimpnet-in-asp-net) 
  - odnosno ubacivanjem Assimp32.dll u direktorijum SysWOW64
  
![image](https://user-images.githubusercontent.com/45834270/101498426-604f6800-396c-11eb-9a44-fe0d7d802c0a.png)

</details>

</details>
