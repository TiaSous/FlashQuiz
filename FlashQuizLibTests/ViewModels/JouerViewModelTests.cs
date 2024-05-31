using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlashQuiz.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlashQuiz.Models;

namespace FlashQuiz.ViewModels.Tests
{
    [TestClass()]
    public class JouerViewModelTests
    {
        [TestMethod()]
        public void RefreshGameTest()
        {
            // Arrange
            var vm = new JouerViewModel();

            // Act
            vm.cardsNotKnow.Add(new Card { Id = 1, Definition = "avoir", Terme = "haben" });
            vm.RefreshGame();

            // Assert
            Assert.AreEqual(vm.cards.Count(), 1);
        }
    }
}