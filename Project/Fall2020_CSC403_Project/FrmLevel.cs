using Fall2020_CSC403_Project.code;
using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Fall2020_CSC403_Project
{
    public partial class FrmLevel : Form
    {
        private Player player;

        private Enemy enemyPoisonPacket;
        private Enemy bossKoolaid;
        private Enemy enemyCheeto;
        private Character[] walls;

        private DateTime timeBegin;
        private FrmBattle frmBattle;
        // Button for Play/Pause of the game.
        private Button buttonForPlayPause;
        private bool gameIsPaused = false;


        // Start of the game code.
        public FrmLevel()
        {
            InitializeComponent();
            InitializeButtonForPlayPause();
            this.KeyPreview = true;
        }
        // The Intialization function for adding the play/pause button for the game.
        private void InitializeButtonForPlayPause()
        {
            buttonForPlayPause = new Button();
            buttonForPlayPause.Size = new Size(60, 30);
            buttonForPlayPause.Text = "Pause";
            buttonForPlayPause.Location = new Point(this.ClientSize.Width - buttonForPlayPause.Width - 10, buttonForPlayPause.Location.Y);
            buttonForPlayPause.TabStop = false;
            buttonForPlayPause.Click += clickButtonForPlayPause;
            this.Controls.Add(buttonForPlayPause);
        }
        private void clickButtonForPlayPause(object sender, EventArgs e)
        {
            gameIsPaused = !gameIsPaused;
            buttonForPlayPause.Text = gameIsPaused ? "Play" : "Pause";
            if (gameIsPaused)
            {
                tmrPlayerMove.Stop();
                tmrUpdateInGameTime.Stop();
            }
            else
            {
                tmrPlayerMove.Start();
                tmrUpdateInGameTime.Start();
            }
        }

        private void FrmLevel_Load(object sender, EventArgs e)
        {
            const int PADDING = 7;
            const int NUM_WALLS = 13;

            player = new Player(CreatePosition(picPlayer), CreateCollider(picPlayer, PADDING));
            bossKoolaid = new Enemy(CreatePosition(picBossKoolAid), CreateCollider(picBossKoolAid, PADDING));
            enemyPoisonPacket = new Enemy(CreatePosition(picEnemyPoisonPacket), CreateCollider(picEnemyPoisonPacket, PADDING));
            enemyCheeto = new Enemy(CreatePosition(picEnemyCheeto), CreateCollider(picEnemyCheeto, PADDING));

            bossKoolaid.Img = picBossKoolAid.BackgroundImage;
            enemyPoisonPacket.Img = picEnemyPoisonPacket.BackgroundImage;
            enemyCheeto.Img = picEnemyCheeto.BackgroundImage;

            bossKoolaid.Color = Color.Red;
            enemyPoisonPacket.Color = Color.Green;
            enemyCheeto.Color = Color.FromArgb(255, 245, 161);

            walls = new Character[NUM_WALLS];
            for (int w = 0; w < NUM_WALLS; w++)
            {
                PictureBox pic = Controls.Find("picWall" + w.ToString(), true)[0] as PictureBox;
                walls[w] = new Character(CreatePosition(pic), CreateCollider(pic, PADDING));
            }

            Game.player = player;
            timeBegin = DateTime.Now;


        }



        private Vector2 CreatePosition(PictureBox pic)
        {
            return new Vector2(pic.Location.X, pic.Location.Y);
        }

        private Collider CreateCollider(PictureBox pic, int padding)
        {
            Rectangle rect = new Rectangle(pic.Location, new Size(pic.Size.Width - padding, pic.Size.Height - padding));
            return new Collider(rect);
        }

        private void tmrUpdateInGameTime_Tick(object sender, EventArgs e)
        {
            TimeSpan span = DateTime.Now - timeBegin;
            string time = span.ToString(@"hh\:mm\:ss");
            lblInGameTime.Text = "Time: " + time.ToString();
        }

        private void tmrPlayerMove_Tick(object sender, EventArgs e)
        {
            // move player
            player.Move();

            // check collision with walls
            if (HitAWall(player))
            {
                player.MoveBack();
            }

            // check collision with enemies
            if (HitAChar(player, enemyPoisonPacket))
            {
                Fight(enemyPoisonPacket);
            }
            else if (HitAChar(player, enemyCheeto))
            {
                Fight(enemyCheeto);
            }
            if (HitAChar(player, bossKoolaid))
            {
                Fight(bossKoolaid);
            }

            // update player's picture box
            picPlayer.Location = new Point((int)player.Position.x, (int)player.Position.y);
        }



        private bool HitAWall(Character c)
        {
            bool hitAWall = false;
            for (int w = 0; w < walls.Length; w++)
            {
                if (c.Collider.Intersects(walls[w].Collider))
                {
                    hitAWall = true;
                    break;
                }
            }
            return hitAWall;
        }

        private bool HitAChar(Character you, Character other)
        {
            return you.Collider.Intersects(other.Collider);
        }

        private void Fight(Enemy enemy)
        {
            player.ResetMoveSpeed();
            player.MoveBack();
            frmBattle = FrmBattle.GetInstance(enemy);
            frmBattle.Show();

            if (enemy == bossKoolaid)
            {
                frmBattle.SetupForBossBattle();
            }
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.ActiveControl = null;
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Handle player movement through ProcessCmdKey
            switch (keyData)
            {
                case Keys.Left:
                    player.GoLeft();
                    break;
                case Keys.Right:
                    player.GoRight();
                    break;
                case Keys.Up:
                    player.GoUp();
                    break;
                case Keys.Down:
                    player.GoDown();
                    break;
                default:
                    return base.ProcessCmdKey(ref msg, keyData); // Important to call the base implementation for other keys
            }

            return true; // Indicates that you've handled the key press
        }


        private void FrmLevel_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                    player.GoLeft();
                    break;

                case Keys.Right:
                    player.GoRight();
                    break;

                case Keys.Up:
                    player.GoUp();
                    break;

                case Keys.Down:
                    player.GoDown();
                    break;

                default:
                    player.ResetMoveSpeed();
                    break;
            }
        }
        private void FrmLevel_KeyUp(object sender, KeyEventArgs e)
        {
            player.ResetMoveSpeed();
        }

        private void lblInGameTime_Click(object sender, EventArgs e)
        {

        }



    }
}
