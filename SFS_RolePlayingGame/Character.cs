using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SFS_RolePlayingGame
{
    public enum JobType
    {
        Warrior,
        Mage
    }
    public class Character
    {
        private static readonly Random rnd = new Random();

        public int Hp { get; set; }
        public int Power { get; set; }
        public int Level { get; set; }
        public string Name { get; set; }

        public virtual void Talk() { Console.WriteLine("[SAY] Character Class 입니다."); }

        public virtual void Attack(Character target)
        {
            int damage = GetRandomDamage(this.Power, 3); // ±3 오차S
            if (target.TryDodge(20)) // 20% 확률 회피
            {
                Console.WriteLine($"[System] {target.Name}이(가) 공격을 회피했다!");
            }
            else
            {
                target.Hp -= damage;
                if (target.Hp < 0) target.Hp = 0;
                Console.WriteLine($"[System] {target.Name}이(가) {damage}의 피해를 입었다!");
            }
        }
        
        // 랜덤 데미지 반환하는 메서드
        public int GetRandomDamage(int basePower, int variance) => basePower + rnd.Next(-variance, variance + 1);
        // 랜덤으로 회피 여부 반환하는 메서드
        public bool TryDodge(int dodgeChancePercent) => rnd.Next(100) < dodgeChancePercent;

    }

    public class Player : Character
    {
        // 플레이어에게 LevelUp() Method를 만들고 overloading
        // - 아무것도 입력 받지 않았다면 적당한 오류 메시지를 반환
        public void LevelUp() { Console.WriteLine("[ERROR] 레벨 업 파라미터를 입력하세요"); }
        // - hp만 입력 받으면 hp 수치만 변경
        public void LevelUp(int hp)
        {
            int oldHp = this.Hp;
            this.Hp = hp;
            this.Level++;
            Console.WriteLine($"[System] {this.Level} Lv로 레벨업 했습니다! (HP {oldHp} -> {this.Hp})");
        }
        // - hp 및 power를 입력 받으면 hp 및 power 수치를 변경
        public void LevelUp(int hp, int power)
        {
            int oldHp = this.Hp;
            int oldPower = this.Power;
            this.Hp = hp;
            this.Power = power;
            this.Level++;
            Console.WriteLine($"[System] {this.Level} Lv로 레벨업 했습니다! (HP {oldHp} -> {this.Hp}, 공격력 {oldPower} -> {this.Power})");
        }
    }

    public class Warrior : Player
    {
        private static readonly string[] lines = {
        "내 검은 약한 자를 용서하지 않는다.",
        "전장은 나의 무대다.",
        "살아서 나를 넘는 자는 없다.",
        "적을 쓰러뜨리기 위해 이 자리에 왔다.",
        "내 검은 약한 자를 용서하지 않는다."
        };

        public Warrior(string name)
        {
            Talk();
            this.Name = name;
            this.Hp = 120;
            this.Level = 1;
            this.Power = 25;
        }

        public override void Talk()
        {
            Random rnd = new Random();
            int idx = rnd.Next(lines.Length);
            Console.WriteLine($"[전사 {Name}] : {lines[idx]}");
        }
    }

    public class Mage : Player
    {
        private static readonly string[] lines = {
        "마력의 힘을 보여주지.",
        "지혜와 마법으로 전장을 지배하겠다.",
        "마법은 혼이 깃든 지혜다.",
        "불꽃은 진실을 태우고, 얼음은 거짓을 얼린다.",
        "나의 주문을 피할 수 있을 것 같은가?"
        };

        public Mage(string name)
        {
            Talk();
            this.Name = name;
            this.Hp = 120;
            this.Level = 1;
            this.Power = 25;
        }

        public override void Talk()
        {
            Random rnd = new Random();
            int idx = rnd.Next(lines.Length);
            Console.WriteLine($"[마법사 {Name}] : {lines[idx]}");
        }
    }

    public class NPC : Character
    {
        private static readonly string[] lines = {
        "정신 안 차려? 넌 지금 실전에 나와 있는 거야!",
        "적을 보면 바로 제압한다. 주저하지 마라!",
        "이게 훈련이었으면 넌 이미 탈락이다!",
        "지금까지 본 훈련병들 중에 니들이 가장 형편없어!",
        "훈련병들, 여기 지금 놀러 왔습니까?"
        };

        public NPC(string name)
        {
            Talk();
            this.Name = name;
            this.Hp = 100;
            this.Level = 1;
            this.Power = 16;
        }

        public override void Talk()
        {
            Random rnd = new Random();
            int idx = rnd.Next(lines.Length);
            Console.WriteLine($"[병장 박아무개] : {lines[idx]}");
        }
    }

    public class Monster : Character
    {

    }

    public class Orc : Monster
    {
        private static readonly string[] lines = {
        "으아악! 피를 원한다!",
        "강한 놈이 이긴다! 그게 규칙이다!",
        "으르르… 널 짓밟아주마!",
        "으르렁! 다 부숴버리겠다!",
        "으아아! 네놈부터 찢어주마!"
        };

        public Orc()
        {
            Talk();
            this.Name = "오크";
            this.Hp = 100;
            this.Level = 1;
            this.Power = 16;
        }


        public override void Talk()
        {
            Random rnd = new Random();
            int idx = rnd.Next(lines.Length);
            Console.WriteLine($"[오크] : {lines[idx]}");
        }
    }

    public class Slime : Monster
    {
        private static readonly string[] lines = {
        "푸르륵… 싸울 준비 됐다.",
        "출렁… 널 녹여주지.",
        "출렁출렁… 널 감쌀 준비 완료.",
        "찌잉… 젤리도 화날 수 있다.",
        "푸룩… 이건 내 구역이다."
        };

        public Slime()
        {
            Talk();
            this.Name = "슬라임";
            this.Hp = 90;
            this.Level = 1;
            this.Power = 20;
        }

        public override void Talk()
        {
            Random rnd = new Random();
            int idx = rnd.Next(lines.Length);
            Console.WriteLine($"[슬라임] : {lines[idx]}");
        }
    }
}
